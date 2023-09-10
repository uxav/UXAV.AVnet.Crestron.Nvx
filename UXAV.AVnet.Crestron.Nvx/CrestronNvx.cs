using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using UXAV.AVnet.Core.DeviceSupport;
using UXAV.Logging;

namespace UXAV.AVnet.Crestron.Nvx
{
    public static class NvxControl
    {
        /// <summary>
        /// Routes stream from one encoder to another.
        /// Automatically sets source as stream on receiver.
        /// This is not supported by "DmNvxD30" and "DmNvxD30C" devices.
        /// </summary>
        /// <param name="encoderIpId">IP ID of encoder</param>
        /// <param name="decoderIpId">IP ID of decoder</param>
        /// <exception cref="ArgumentException">IP ID does not exist in system</exception>
        /// <exception cref="OperationCanceledException">Routing operation failed</exception>
        public static void RouteStream(uint encoderIpId, uint decoderIpId)
        {
            if (!CipDevices.ContainsDevice(encoderIpId))
                throw new ArgumentException($"No device with ID 0x{encoderIpId:X2}", nameof(encoderIpId));
            if (!CipDevices.ContainsDevice(decoderIpId))
                throw new ArgumentException($"No device with ID 0x{decoderIpId:X2}", nameof(decoderIpId));
            try
            {
                var tx = CipDevices.GetDevice<DmNvxBaseClass>(encoderIpId);
                var rx = CipDevices.GetDevice<DmNvxBaseClass>(decoderIpId);
                rx.Control.ServerUrl.StringValue = tx.Control.ServerUrlFeedback.StringValue;
                rx.Control.VideoSource = eSfpVideoSourceTypes.Stream;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException(
                    "Cancelled due to a problem routing the stream. See inner exception", e);
            }
        }

        /// <summary>
        /// Select source input on encoder or decoder types
        /// This method is not supported by DMF-XX-4K-SFP,"DmNvxD30","DmNvxD30C","DmNvxE30","DmNvxE30C","DmNvxE31" and "DmNvxE31C" devices.
        /// </summary>
        /// <param name="ipId">IP ID of encoder / decoder</param>
        /// <param name="source"><see cref="eSfpVideoSourceTypes"/>The source of the input</param>
        /// <exception cref="ArgumentException">IP ID does not exist in system</exception>
        /// <exception cref="OperationCanceledException">Routing operation failed</exception>
        public static void SetSource(uint ipId, eSfpVideoSourceTypes source)
        {
            if (!CipDevices.ContainsDevice(ipId))
                throw new ArgumentException($"No device with ID 0x{ipId:X2}", nameof(ipId));
            try
            {
                var device = CipDevices.GetDevice<DmNvxBaseClass>(ipId);
                device.Control.VideoSource = source;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException(
                    "Cancelled due to a problem setting the source. See inner exception", e);
            }
        }

        /// <summary>
        /// Selects the video source from choice of subscribed streams
        /// </summary>
        /// <param name="decoderIpId">IP ID of decoder</param>
        /// <param name="stream">Subscribed stream index</param>
        /// <exception cref="ArgumentException">IP ID does not exist in system</exception>
        /// <exception cref="OperationCanceledException">Routing operation failed</exception>
        public static void SelectSubscribedStream(uint decoderIpId, ushort stream)
        {
            if (!CipDevices.ContainsDevice(decoderIpId))
                throw new ArgumentException($"No device with ID 0x{decoderIpId:X2}", nameof(decoderIpId));
            try
            {
                var device = CipDevices.GetDevice<DmNvxBaseClass>(decoderIpId);
                device.XioRouting.VideoOut.UShortValue = stream;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException(
                    "Cancelled due to a problem selecting the stream. See inner exception", e);
            }
        }

        public static void SelectSubscribedStream(uint decoderIpId, string streamName)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                SelectSubscribedStream(decoderIpId, 0);
                return;
            }

            var streams = GetSubscribedStreams(decoderIpId);
            if (streams.All(s => s.Value.SessionNameFeedback.StringValue != streamName))
            {
                throw new ArgumentException("Steam name not found", nameof(streamName));
            }

            var stream = GetSubscribedStreams(decoderIpId)
                .First(s => s.Value.SessionNameFeedback.StringValue == streamName).Key;
            SelectSubscribedStream(decoderIpId, stream);
        }

        public static Dictionary<ushort, DmNvxBaseClass.DmNvx35xStreamInfo> GetSubscribedStreams(uint decoderIpId)
        {
            if (!CipDevices.ContainsDevice(decoderIpId))
                throw new ArgumentException($"No device with ID 0x{decoderIpId:X2}", nameof(decoderIpId));
            return GetSubscribedStreams(CipDevices.GetDevice<DmNvxBaseClass>(decoderIpId));
        }

        public static Dictionary<ushort, DmNvxBaseClass.DmNvx35xStreamInfo> GetSubscribedStreams(DmNvxBaseClass decoder)
        {
            var list = new Dictionary<ushort, DmNvxBaseClass.DmNvx35xStreamInfo>();
            if (decoder.Control.DeviceModeFeedback == eDeviceMode.Transmitter)
                throw new ArgumentException($"Device \"{decoder}\" is in transmitter mode");
            try
            {
                var numberOfStreams = decoder.XioRouting.SubscribeStreams.SubscribedDevicesFeedback.UShortValue;
                for (ushort count = 1; count <= numberOfStreams; count++)
                {
                    var stream = decoder.XioRouting.SubscribeStreams.StreamInfo[count];
                    list.Add(count, stream);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return list;
        }

        public static DmNvxBaseClass GetOrCreateEndpoint(string typeName, uint ipId, string description)
        {
            if (CipDevices.ContainsDevice(ipId)) return CipDevices.GetDevice<DmNvxBaseClass>(ipId);
            var value = typeof(DmNvxBaseClass).Namespace;
            if (value == null || typeName.StartsWith(value))
                return (DmNvxBaseClass)CipDevices.CreateDevice(typeName, ipId, description);
            var fullName = typeof(DmNvxBaseClass).Namespace + "." + typeName;
            return (DmNvxBaseClass)CipDevices.CreateDevice(fullName, ipId, description);
        }

        public static DmNvxBaseClass[] GetRoutedEndpoints(uint streamIndex)
        {
            var results = new List<DmNvxBaseClass>();
            foreach (var endpoint in CipDevices.GetDevices<DmNvxBaseClass>()
                         .Where(endpoint => endpoint.Control.DeviceModeFeedback == eDeviceMode.Receiver))
            {
                if (endpoint.XioRouting.VideoOutFeedback.UShortValue != streamIndex || streamIndex <= 0) continue;
                if (endpoint is DmNvx350 && endpoint.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Stream)
                {
                    results.Add(endpoint);
                }
                else if (!(endpoint is DmNvx350))
                {
                    results.Add(endpoint);
                }
            }

            return results.ToArray();
        }

        public static DmNvxBaseClass[] GetRoutedEndpoints(string streamName)
        {
            var results = new List<DmNvxBaseClass>();
            foreach (var endpoint in CipDevices.GetDevices<DmNvxBaseClass>()
                         .Where(endpoint => endpoint.Control.DeviceModeFeedback == eDeviceMode.Receiver))
            {
                if (endpoint.XioRouting.VideoOutFeedback.UShortValue <= 0) continue;
                var routingInput = endpoint.XioRouting.Input[endpoint.XioRouting.VideoOutFeedback.UShortValue];
                if (routingInput.NameFeedback.StringValue != streamName) continue;
                if (endpoint is DmNvx35x dmNvx35X &&
                    dmNvx35X.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Stream)
                {
                    results.Add(endpoint);
                }
                else if (!(endpoint is DmNvx35x))
                {
                    results.Add(endpoint);
                }
            }

            return results.ToArray();
        }

        public static DmNvxBaseClass GetEndpoint(uint deviceIpId)
        {
            if (!CipDevices.ContainsDevice(deviceIpId))
                throw new ArgumentException($"No device with ID 0x{deviceIpId:X2}", nameof(deviceIpId));
            return CipDevices.GetDevice<DmNvxBaseClass>(deviceIpId);
        }

        public static string GetDmInputEventIdName(int value)
        {
            var type = typeof(DMInputEventIds);
            foreach (var field in type.GetFields())
            {
                if (field.FieldType != typeof(int)) continue;
                var v = (int)field.GetValue(null);
                if (v == value) return field.Name;
            }

            return "Unknown ID " + value;
        }
    }
}
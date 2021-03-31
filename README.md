# UXAV.CrestronNvx

Template project contents to be replaced.

---

## TEMPLATE USAGE

- Create a new project from this template.
  This givees you a fresh repository using the latest contents of this template.
  
- Wait for the GitHub Action to run once on the initial commit.
  This will run the scripts to replace the file naming and contents with your new project name.
  
- Remove the scripts below. Once you've created a project from this template you should remove the workflow
  which replaces the contents of the template on first run of the GitHub Action. 
    
- Remove the following files:
  `/.github/workflows/replace_template_contents.yml` & `/.github/scripts/ReplaceContents.ps1`
  
 ### Notes on project naming

Use a project name such as `MyNewProject`

Using a project name such as `my-new-project` is ok but the .NET solution and related files will be formatted
automatically as `MyNewProject`.

---

## Dependencies

The project uses the following dependencies:

- [UXAV.AVnetCore](https://github.com/uxav/AVnetCore)
  Core library
- [UXAV.Logging](https://github.com/uxav/UXAV.Logging)
  Logging and console application
- [Crestron.SimplSharp.SDK.ProgramLibrary](https://www.nuget.org/packages/Crestron.SimplSharp.SDK.ProgramLibrary/)
  Core SDK required for Crestron (see [Crestron Licensing](#crestron-licensing) below)

## Project Information

Insert project document info here

## Crestron Licensing

This project contains tools and libraries which directly fall under licensing arrangements from Crestron.

Specifically:

By downloading, installing, or otherwise using Crestron's software development tools ('Software Tools'),
which includes both Crestron Software and Third Party Software, as defined below.
You represent that You are authorized by Crestron under a separate written agreement to access and use
these Software Tools and that You further agree to be bound by the terms of this license agreement
(the 'Agreement'), which is a legal contract between You (either an individual or a single business entity)
and Crestron Electronics, Inc. 

If You do not agree to the terms of this Agreement, do not install or use the Software Tools.

Further details are available from [Crestron](https://www.crestron.com/contact/have-a-question)

## Project Licensing

Copyright (c) 2021 UXAV Solutions Limited

Permission is hereby granted, to any person obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software for the continued use and development of the system on which it was installed
originally, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

- The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

- Any persons obtaining the software have no rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software without written persmission from UXAV Solutions Limited, if it is not for use on the system on which it
  was originally installed.

# WPF Custom Dialog Models

This project contains models to create custom dialogs for use by a WPF 
application or another DLL assembly. Both of the example implementations are in 
the same DLL, showing that custom dialogs can be in individual DLLs or packaged 
as a suite of related dialogs in a single DLL.

The parent collection can be found at 
[OSNW-WPF-Custom-Dialog-Models](https://github.com/OldSchoolNewWorld/OSNW-WPF-Custom-Dialog-Models).  
The consumer model can be found at 
[Consumer](https://github.com/OldSchoolNewWorld/OSNW-WPF-Custom-Dialog-Models/tree/master/Consumer).  

## Overview

These are models. They are intended to provide source code that can be copied 
and used to create new custom dialogs. These models are not intended to be used 
directly or as base classes to create new custom dialogs. The DLL 
(ex. Models.dll) created by this project is matched to the application model; 
it is not intended to be referenced by a project based on this model. Only the 
.vb and .xaml files are expected to be of actual use in the creation of new 
custom dialogs. The steps described below will create the other files.

Items marked "(ex. *ItemName*)" in this README file indicate the values that 
match the examples in the dialog and application models.

### Coding Notes

"Option Explicit On", "Option Strict On", and "Option Infer Off" are set in the 
code to make it clear what is being done and to make it easier to research the 
code elements. Fully qualified references are used in much of the code, 
including the use of "Me".

Some of the specific implementation details are included in the models as XML 
comments. Those XML comments are targeted at developers and are left in place 
so that they are visible to IntelliSense while creating a new 
application/assembly. Items marked "DEV:" are intended for a developer using 
the models, not for visibility to consuming assemblies that use the dialog. 
They can be left in place, deleted, suppressed by adding a fourth apostrophe, 
or suppressed from output by the compiler via "Generate XML documentation file" 
in the project Properties.

External research references are marked "REF:". Those are links to research
done while looking for code samples or detailed explanations of properties and 
methods.

The models include regions. Those regions may appear to be overkill for the 
simplistic example but can provide organization for a more complex dialog. In a 
complex dialog, the region content may be worth moving to separate modules.

As with regions, the use of subroutines may appear to be overkill for the 
simplistic example but can provide value for a more complex dialog. In a 
complex dialog, the detailed content may be worth moving to separate regions or 
modules. `Window_Loaded()` and `DoWindow_Loaded()` are examples of how that can 
be used to minimize the code shown in a group of event handlers. The call to a 
known-good subroutine is something that can easily be stepped over while 
debugging. Due to the cost of setting up a subroutine call, calls to even small 
subroutines should probably be avoided if that is likely to happen in a large 
loop. When expected to be used in a loop, it may be better to bring the code 
into the calling routine.

### Window in a DLL

*Like* an internal dialog (as shown  in the associated demo/test application), 
*all* public features of `System.Windows.Window` are reachable by the consuming 
application/assembly. *Unlike* an internal dialog, and as shown in the 
associated demo/test application, a custom dialog embedded in a DLL can be 
consumed by any assembly.

### Hosted Dialog

A `DialogHost` creates a layer of abstraction between its underlying 
`HostedDialogWindow` and the consuming assembly. Unlike a window in a DLL, *no* 
public features of `System.Windows.Window` are reachable by the consuming 
application/assembly unless exposed by the hosting object.

## Implement one or more dialogs in a DLL, ...

- Create a solution (ex. OSNW WPF Custom Dialog Models), via "Create a new 
project", or open an existing solution.
- Add a new "WPF Class Library" (ex. Models) for the DLL to (ex. OSNW WPF 
Custom Dialog Models).
  - Delete the default Class1.
- Set the project properties for (ex. Models).
  - Assembly name
  - Default namespace
- Proceed below to add either a dialog window or a hosted dialog to 
(ex. Models).

### For a window in a DLL ...

- Add a "New Item" - "Window (WPF)" (ex. DialogWindow.xaml) to (ex. Models).
- Add resource files (icon, etc.).
  - Set "Build Action" to "Resource".
  - Set "Copy to Output Directory" to "Do not copy".
- Populate the grid.
- Set the window features (title, icon, etc.).
- Populate the window code file(s).

### For a hosted dialog, ...

- Add a "New Item" - "Class" (ex. DialogHost.vb) to (ex. Models) - Class for 
the host object.
- Add a "New Item" - "Window (WPF)" (ex. HostedWindow.xaml) for the hosted 
window.
- Add resource files (icon, etc.).
  - Set "Build Action" to "Resource".
  - Set "Copy to Output Directory" to "Do not copy".
- Populate the grid.
- Set the window features (title, icon, etc.).
  - Note the use of "x:ClassModifier="Friend"".
- Populate the window code file(s).
- Populate the host code file(s).

### Icons

There are several ways to assign a dialog's icon. See the related comments in 
the code files.
- Assign via a fixed/initial icon in the XAML
  - This is included by default in all of the example dialogs, but can be 
deleted or overridden if so desired. This is probably the most likely approach 
for an embedded dialog. The designated icon will be used when the dialog is 
shown.
  - The (ex. Dialog.ico) icon included in the sample was generated from the 
"Dialog.png" file found at [Visual Studio Image 
Library](https://www.microsoft.com/en-us/download/details.aspx?id=35825).
  - The XAML syntax for the embedded dialog model is different from the syntax 
for the window in a DLL and hosted dialog models.
  - Add the icon file (ex. Dialog.ico) to the dialog or application project.
  - Set "Build Action" for (ex. Dialog.ico) to "Resource".
  - Set "Copy to Output Directory" for (ex. Dialog.ico) to "Do not copy".
- Assign in the window's initialization (`Window_Initialized`/`Window_Loaded`) 
code.
  - This is, effectively, the same as assigning the icon in the XAML. It may be 
useful if something in the operation warrants a change in appearance based on 
state.
  - This approach could be taken if there is a desire to load an icon from a 
file that might be modified externally.
	- Example implementations are shown in `OSNW.Dialog.DialogHost.New()`.
- Copy the application icon
  - This might be done if consuming software wants internal dialogs, or 
third-party dialogs from a DLL, to display the application icon on the dialog 
itself and in the taskbar. That can, in turn, be blocked by the creator of a 
hosted dialog by not exposing `Icon` in the `DialogHost`.
	- An example implementation is shown in `HostedDialogButton_Click()` in the 
sample application.
- Assign by the consuming code
  - This is probably pointless for the embedded model except for something 
related to changes that reflect state.
  - In the hosted dialog case, apply that to the host, which is all that is 
reachable.
	- Example implementations are shown in `OSNW.Dialog.DialogHost.New()`.
- Copy a custom overridable icon
  - The XAML syntax for the embedded dialog model is different from the syntax 
for the window in a DLL and hosted dialog models.
  - Add the icon file (ex. Dialog.ico) to the dialog or application project.
  - Set "Build Action" for (ex. Dialog.ico) to "None".
  - Set "Copy to Output Directory" for (ex. Dialog.ico) to "Copy if newer".
  - Copy any icon file, prior to starting the application and with the expected 
name, to replace the original icon file.
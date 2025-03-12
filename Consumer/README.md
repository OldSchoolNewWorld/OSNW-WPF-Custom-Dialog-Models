# Demo/Test WPF Custom Dialog Models

This project is a WPF application model to test or demonstrate the 
consumption of dialogs based on the OSNW WPF Custom Dialog Models. It includes 
an embedded duplicate of the example dialog used in the models.  

The parent collection can be found at 
[OSNW-WPF-Custom-Dialog-Models](https://github.com/OldSchoolNewWorld/OSNW-WPF-Custom-Dialog-Models).  
The dialog models can be found at 
[Models](https://github.com/OldSchoolNewWorld/OSNW-WPF-Custom-Dialog-Models/tree/master/Models).  

## Overview

This is a model. It is intended to be copied, in whole or in part, to create 
new custom assemblies that contain or reuse custom dialogs.

The main window contains three buttons that show custom dialogs. The leftmost 
button shows a dialog that is built into the application (or any assembly in 
which it is defined) itself. The middle button shows a dialog that is embedded 
in a DLL and constructed as a window. The rightmost button shows a dialog that 
is embedded in a DLL and constructed as a shell that shows the actual dialog 
window. The main window also contains items that reflect the result of showing 
the dialog.

Items marked "(ex. *ItemName*)" in this README file indicate the values that 
match the examples in the dialog and application models.

### Coding Notes

"Option Explicit On", "Option Strict On", and "Option Infer Off" are set in the 
code to make it clear what is being done and to make it easier to research the 
code elements. Fully qualified references used in much of the code, including 
the use of "Me".

Some of the XML comments are targeted at developers and are left in place so 
that they are visible to IntelliSense while creating a new application/assembly.
Items marked "DEV:" are intended for a developer using the model, not for
consuming assemblies that use the dialog. They can be left in place, deleted,
suppressed by adding another apostrophe, or suppressed from output by the
compiler via "Generate XML documentation file".

External research references are marked "REF:". Those are links to research 
done while looking for code samples or detailed explanations of properties and 
methods.

The model includes regions. Those regions may appear to be overkill for the 
simplistic example dialog but can provide organization for a more complex 
dialog. In a more complex dialog, the region content may be worth moving to 
separate modules.

As with regions, the use of subroutines may appear to be overkill for the 
simplistic example dialog but can provide value for a more complex embedded 
dialog or application. In a complex implementation, the detailed content may be 
worth moving to separate regions or modules. Subroutines can be used to 
minimize the code shown in a group of event handlers. The call to a known-good 
subroutine is something that can easily be stepped over while debugging. Due to 
the cost of setting up a subroutine call, calls to even small subroutines 
should probably be avoided if that is likely to happen in a large loop. When 
expected to be used in a loop, it may be better to bring the code into the 
calling routine.

### Locally Embedded Dialog

A locally embedded dialog window is part of the application, or the assembly in 
which it is defined. As such, it is dedicated to use by the defining 
application/assembly. *All* public features of `System.Windows.Window` are 
reachable by the application/assembly.

### Window in a DLL

*Like* a locally embedded dialog, *all* public features of 
`System.Windows.Window` are reachable by the consuming application/assembly. 
*Unlike* a locally embedded dialog, a custom dialog embedded in a DLL can be 
consumed by any assembly.

### Hosted Dialog

A `DialogHost` creates a layer of abstraction between its underlying 
`HostedDialogWindow` and the consuming assembly.

Unlike a window in a DLL, a `HostedDialogWindow` is hosted by a shell 
`System.Object` that can be consumed by any assembly. `HostedDialogWindow` is 
designated as`Friend`; it is only directly available to the associated 
`DialogHost`.

Unlike a window in a DLL, *no* public features of `System.Windows.Window` are 
reachable by the consuming application/assembly unless exposed by the hosting 
object. Public members of `System.Windows.Window` are not reachable by the 
consuming assembly unless exposed by the DialogHost.

## Implement an application that consumes a dialog in a DLL

- Create a solution, via "Create a new project", or open an existing solution.
- Add a new "WPF Application" project (ex. Consumer) to the solution.
- Set the project properties for (ex. Consumer).
- Set the "Project Dependencies" for the solution.
- Set "Dependencies" for Consumer to include the DLL with the dialog(s) (ex.  Models.DLL).
- Fill in the XAML.
  - Populate the grid.
  - Set the window features (title, icon, etc.).
  - Populate the window code file(s).

## Implement a dialog embedded in an assembly

- Add a "New Item" - "Window (WPF)" (ex. EmbeddedWindow.xaml), for the embedded 
window, to (ex. Consumer).
- Add resource files (icon, etc.).  
  - Set "Build Action" to "Resource".  
  - Set "Copy to Output Directory" to "Do not copy".
- Fill in the XAML.
  - Populate the grid.
  - Set the window features (title, icon, etc.).
  - Populate the window code file(s).
- Update the Project Dependencies.
- Set the Startup Project for the solution.

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
- Copy a custom overridable icon
  - The XAML syntax for the embedded dialog model is different from the syntax 
for the window in a DLL and hosted dialog models.
  - Add the icon file (ex. Dialog.ico) to the dialog or application project.
  - Set "Build Action" for (ex. Dialog.ico) to "None".
  - Set "Copy to Output Directory" for (ex. Dialog.ico) to "Copy if newer".
  - Copy any icon file, prior to starting the application and with the expected 
name, to replace the original icon file.
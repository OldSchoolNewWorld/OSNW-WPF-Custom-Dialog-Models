Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Windows

' NOTE: <UseWPF>true</UseWPF> may need to be added to the dialogs'
' <projectname>.vbproj file.
'   https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props-desktop
'   Maybe just include PresentationFramework.dll? Or System.Windows?

''' <summary>
''' DEV: Represents a model for a shell that exposes minimal features of a WPF
''' dialog displayed as a <see cref="System.Windows.Window"/>.
''' </summary>
''' <remarks>
''' DEV:<para>
''' A <c>DialogHost</c> creates a layer of abstraction between its underlying
''' <see cref="HostedWindow"/> and the consuming assembly.
''' <see cref="HostedWindow"/> is designated as <c>Friend</c> and its XAML
''' contains <c>x:ClassModifier="Friend"</c>; it is only directly available to
''' the associated <c>DialogHost</c>. Public members of
''' <see cref="System.Windows.Window"/> are not reachable by the consuming
''' assembly unless exposed by the <c>DialogHost</c>.
''' </para>
''' <para>
''' <c>DialogHost</c> is marked "NotInheritable" because it is intended as a
''' model, not as a base type. It is created as a reusable (available from a
''' DLL) class that hosts a specific dialog window. It is not dedicated to
''' consumption by any particular assembly.
''' </para>
''' <para>
''' A <c>DialogHost</c> is a shell that isolates the window itself, hiding most
''' features of System.Windows.Window. Necessary System.Windows.Window features
''' can be exposed as pass-through accessors.
''' </para>
''' <para>
''' This class would need to expose certain Window properties and methods to be
''' used by the calling assembly. Items likely to be desirable for access from
''' the consuming assembly include: Icon, Owner, ShowInTaskbar, Title,
''' WindowStartupLocation, ShowDialog(), and DialogResult.
''' </para>
''' <example> This sample shows how to use a <c>DialogHost</c>. NOTE:
''' "OSNW.Dialog.DialogHost" only refers to the model included here; it is not
''' intended as a base type. Change the name to suit the the new implementation.
''' <code>
''' 
''' Imports OSNW.Dialog
''' 
''' ' Set up the dialog.
''' Dim Dlg As New OSNW.Dialog.DialogHost With {
'''     .Owner = Me,
'''     .ShowInTaskbar = False,
'''     .Title = "Dialog Hosted by a Class",
'''     .WindowStartupLocation =
'''         System.Windows.WindowStartupLocation.CenterScreen}
''' 
''' ' Show the dialog. Process the result.
''' Dim DlgResult As System.Boolean? = Dlg.ShowDialog
''' If DlgResult Then
''' 
'''     ' Extract any data being returned.
''' 
'''     ' Update the visuals.
''' 
'''     'Else
'''     '' Is anything needed when ShowDialog is false?
''' End If
''' 
''' </code>
''' </example>
''' </remarks>
Public NotInheritable Class DialogHost

#Region "Properties"

    ' DEV: These specific properties are not intended as part of the model. They
    ' are included to support operation of the example. In general, examination
    ' by the setter should normally be handled here before passing data to the
    ' window.
    Public Property Red As System.Byte
    Public Property Green As System.Byte
    Public Property Blue As System.Byte
    Public Property TheString As System.String
    Public Property TheInteger As System.Int32

#End Region ' "Properties"

#Region "Model pass-through properties"
    ' These are properties for a HostedDialogWindow that does not always exist.
    ' They are passed to the Window when it gets created.

    Private m_DialogResult As System.Boolean?
    ''' <summary>
    ''' Gets or sets the dialog result value, which is the value that is
    ''' returned from the System.Windows.Window.ShowDialog method.
    ''' </summary>
    ''' <returns>
    ''' A System.Nullable`1 value of type System.Boolean. The default is false.
    ''' </returns>
    ''' <exception cref="System.InvalidOperationException">
    ''' System.Windows.Window.DialogResult is set before a window is opened by
    ''' calling System.Windows.Window.ShowDialog. -or-
    ''' System.Windows.Window.DialogResult is set on a window that is opened by
    ''' calling System.Windows.Window.Show.
    ''' </exception>
    <System.ComponentModel.DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    <System.ComponentModel.TypeConverter(GetType(DialogResultConverter))>
    Public Property DialogResult As System.Boolean?
        Get
            Return m_DialogResult
        End Get
        Set(value As System.Boolean?)
            m_DialogResult = value
        End Set
    End Property

    Private m_Icon As System.Windows.Media.ImageSource
    ''' <summary>
    ''' Gets or sets a window's <c>Icon</c>.
    ''' </summary>
    ''' <returns>
    ''' A System.Windows.Media.ImageSource object that represents the icon.
    ''' </returns>
    ''' <remarks>DEV: The HostedDialogWindow has a default icon set to
    ''' "Dialog.ico". Use the <c>Icon</c> property to override it.</remarks>
    Property Icon As System.Windows.Media.ImageSource
        Get
            Return Me.m_Icon
        End Get
        Set(value As System.Windows.Media.ImageSource)
            Me.m_Icon = value
        End Set
    End Property

    Private m_Owner As System.Windows.Window
    ' REF: How do I write [DefaultValue(null)] in VB.NET?
    '   <DefaultValue(Nothing)> does not compile
    ' https://stackoverflow.com/questions/29748703/how-do-i-write-defaultvaluenull-in-vb-net-defaultvaluenothing-does-not
    ''' <summary>
    ''' Gets or sets the <see cref="System.Windows.Window"/> that owns this
    ''' <see cref="DialogHost"/>.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="System.Windows.Window"/> object that represents the owner
    ''' of this <see cref="DialogHost"/>.
    ''' </returns>
    ''' <exception cref="System.ArgumentException">
    ''' A window tries to own itself -or- Two windows try to own each other.
    ''' </exception>
    ''' <exception cref="System.InvalidOperationException">
    ''' The <see cref="System.Windows.Window.Owner"/> property is set on a
    ''' visible window shown using
    ''' <see cref="System.Windows.Window.ShowDialog"/> -or- The
    ''' <see cref="System.Windows.Window.Owner"/> property is set with a window
    ''' that has not been previously shown.
    ''' </exception>
    <DefaultValue(DirectCast(Nothing, Object))>
    Public Property Owner As System.Windows.Window
        Get
            Return Me.m_Owner
        End Get
        Set(value As System.Windows.Window)
            Me.m_Owner = value
        End Set
    End Property

    Private m_ResizeMode As System.Windows.ResizeMode
    ''' <summary>
    ''' Gets or sets a value that indicates how a window is resized.
    ''' </summary>
    ''' <returns>A <see cref="System.Windows.ResizeMode"/> value specifying the
    ''' resize mode.</returns>
    Public Property ResizeMode As System.Windows.ResizeMode
        Get
            Return Me.m_ResizeMode
        End Get
        Set(value As System.Windows.ResizeMode)
            Me.m_ResizeMode = value
        End Set
    End Property

    Private m_ShowInTaskbar As System.Boolean
    ''' <summary>
    ''' Gets or sets a value that indicates whether the window has a task bar
    ''' button.
    ''' </summary>
    ''' <returns>
    ''' <c>True</c> if the window has a task bar button; otherwise,
    ''' <c>False</c>. Does not apply when the window is hosted in a browser.
    ''' </returns>
    Public Property ShowInTaskbar As System.Boolean
        Get
            Return Me.m_ShowInTaskbar
        End Get
        Set(value As System.Boolean)
            Me.m_ShowInTaskbar = value
        End Set
    End Property

    Private m_Title As System.String
    ''' <summary>
    ''' Gets or sets a <see cref="System.Windows.Window"/>'s title.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="System.String"/> that contains the window's title.
    ''' </returns>
    <Localizability(LocalizationCategory.Title)>
    Public Property Title As System.String
        Get
            Return Me.m_Title
        End Get
        Set(value As System.String)
            Me.m_Title = value
        End Set
    End Property

    Private m_WindowStartupLocation As WindowStartupLocation
    ''' <summary>
    ''' Gets or sets the position of the <see cref="DialogHost"/>'s window when
    ''' first shown.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="System.Windows.WindowStartupLocation"/> value that
    ''' specifies the top/left position of a window when first shown. The
    ''' default is <see cref="System.Windows.WindowStartupLocation.Manual"/>.
    ''' </returns>
    <System.ComponentModel.DefaultValue(
        System.Windows.WindowStartupLocation.Manual)>
    Public Property WindowStartupLocation As WindowStartupLocation
        Get
            Return Me.m_WindowStartupLocation
        End Get
        Set(value As WindowStartupLocation)
            Me.m_WindowStartupLocation = value
        End Set
    End Property

#End Region ' "Model pass-through properties"

#Region "Exception handling"

    ''' <summary>
    ''' Reports an invalid call to one of the
    ''' <c>ShowExceptionMessageBox(&lt;varies&gt;)</c> implementations.
    ''' </summary>
    ''' <param name="paramName">Specifies the name of the parameter that was
    ''' invalid.</param>
    ''' <param name="reason">Specifies the reason for the rejection.</param>
    ''' <remarks>
    ''' This is for invalid calls to
    ''' <c>ShowExceptionMessageBox(&lt;varies&gt;)</c>, not for generic invalid
    ''' procedure calls.
    ''' Note: A <c>DialogHost</c> is not a <c>Window</c>; it cannot be used as
    ''' the <c>owner</c> of the <c>MessageBox</c>. The reference here is changed
    ''' to <c>Me.Owner</c>, which is a <c>Window</c>.
    ''' </remarks>
    Private Sub ShowExceptionArgNotice(ByVal paramName As System.String,
                                       ByVal reason As System.String)

        ' REF: OSNW-Exception-Handling-Models
        ' https://github.com/OldSchoolNewWorld/OSNW-Exception-Handling-Models

        Dim CaptionStr As System.String = "Invalid ShowExceptionMessageBox"
        Dim IntroDetails As System.String = System.String.Concat(
            "An invalid exception notice was requested.",
            $" There is a problem with '{paramName}'.")

        ' Construct and show the notice.
        Dim ShownDetail As System.String = System.String.Concat(IntroDetails,
            System.Environment.NewLine, System.Environment.NewLine, reason)
        If Me.Owner Is Nothing Then
            ' Show without Me.Owner.
            ' REF: https://learn.microsoft.com/en-us/dotnet/api/system.windows.messagebox.show?view=windowsdesktop-9.0#remarks
            ' Use an overload of the Show method, which enables you to specify
            ' an owner window. Otherwise, the message box is owned by the window
            ' that is currently active.
            System.Windows.MessageBox.Show(ShownDetail, CaptionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        Else
            ' Use Me.Owner.
            System.Windows.MessageBox.Show(Me.Owner, ShownDetail, CaptionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        End If

    End Sub ' ShowExceptionArgNotice

    ''' <summary>
    ''' Provides a consistent generic appearance for messages.
    ''' </summary>
    ''' <param name="captionStr">Specifies the caption to show on the 
    ''' <c>MessageBox</c>.</param>
    ''' <param name="introDetails">Specifies a summary of the exception.</param>
    ''' <param name="techDetails">Specifies detailed information about the 
    ''' exception.</param>
    ''' <remarks>
    ''' Note: A <c>DialogHost</c> is not a <c>Window</c>; it cannot be used as
    ''' the <c>owner</c> of the <c>MessageBox</c>. The reference here is changed
    ''' to <c>Me.Owner</c>, which is a <c>Window</c>.
    ''' </remarks>
    Private Sub ShowExceptionNotice(ByVal captionStr As System.String,
        ByVal introDetails As System.String, ByVal techDetails As System.String)

        ' REF: OSNW-Exception-Handling-Models
        ' https://github.com/OldSchoolNewWorld/OSNW-Exception-Handling-Models

        ' Construct and show the notice.
        Dim ShownDetail As System.String = System.String.Concat(introDetails,
            System.Environment.NewLine, System.Environment.NewLine, techDetails)
        If Me.Owner Is Nothing Then
            ' Show without Me.Owner.
            ' REF: https://learn.microsoft.com/en-us/dotnet/api/system.windows.messagebox.show?view=windowsdesktop-9.0#remarks
            ' Use an overload of the Show method, which enables you to specify
            ' an owner window. Otherwise, the message box is owned by the window
            ' that is currently active.
            System.Windows.MessageBox.Show(ShownDetail, captionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        Else
            ' Use Me.Owner.
            System.Windows.MessageBox.Show(Me.Owner, ShownDetail, captionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        End If

    End Sub ' ShowExceptionNotice

    ''' <summary>
    ''' Shows details about an <c>Exception</c> that was caught.
    ''' </summary>
    ''' <param name="caughtBy">Specifies the process in which an exception was
    ''' caught.</param>
    ''' <param name="caughtEx">Provides the exception that was caught.</param>
    Private Sub ShowExceptionMessageBox(
        ByVal caughtBy As System.Reflection.MethodBase,
        ByVal caughtEx As System.Exception)

        ' REF: OSNW-Exception-Handling-Models
        ' https://github.com/OldSchoolNewWorld/OSNW-Exception-Handling-Models

        ' Argument checking.
        If caughtEx Is Nothing Then
            Me.ShowExceptionArgNotice(NameOf(caughtEx),
                $"'{NameOf(caughtEx)}' cannot be 'Nothing'/'Null'.")
            Exit Sub ' Early exit.
        End If
        Dim CaughtByName As System.String = If(caughtBy Is Nothing,
            $"Unspecified '{NameOf(caughtBy)}'", caughtBy.Name)

        ' Gather information of interest.
        Dim CaughtExMessage As System.String = caughtEx.Message
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException
        Dim IntroDetails As System.String =
            $"'{CaughtByName}' failed with message '{CaughtExMessage}'."
        Dim TechDetails As System.String =
            $"The initial cause is {CaughtExBaseException}."

        ' Construct and show the notice.
        Dim Caption As System.String = "Process Failure"
        Me.ShowExceptionNotice(Caption, IntroDetails, TechDetails)

    End Sub ' ShowExceptionMessageBox

#End Region ' "Exception handling"

#Region "Constructor helpers"

    ''' <summary>
    ''' A helper class to convert image data.
    ''' </summary>
    Private Class IcoToBitmapSourceConverter

        ' REF: From AI in Edge. No reference shown.
        ' https://www.bing.com/search?pglt=297&q=.net+ico+to+bitmapsource&cvid=fe43db60a0ed49669c0b4c314e6fa0d6&gs_lcrp=EgRlZGdlKgYIABBFGDkyBggAEEUYOTIGCAEQABhAMgYIAhAAGEAyBggDEAAYQDIGCAQQABhAMgYIBRAAGEAyBggGEAAYQDIGCAcQABhAMgYICBAAGEDSAQkyNzI4N2owajGoAgCwAgA&FORM=ANNTA1&PC=DCTS
        '
        ' using System;
        ' using System.IO;
        ' using System.Windows.Media.Imaging;
        ' 
        ' public class IcoToBitmapSourceConverter
        ' {
        '     public static BitmapSource ConvertIcoToBitmapSource(string icoFilePath)
        '     {
        '         if (string.IsNullOrEmpty(icoFilePath))
        '         {
        '             throw new ArgumentException("ICO file path cannot be null or empty.", nameof(icoFilePath));
        '         }
        ' 
        '         using (FileStream icoStream = new FileStream(icoFilePath, FileMode.Open, FileAccess.Read))
        '         {
        '             BitmapDecoder decoder = BitmapDecoder.Create(icoStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        '             return decoder.Frames[0];
        '         }
        '     }
        ' }

        ''' <summary>
        ''' Converts an icon file, typically *.ico, to a
        ''' <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
        ''' </summary>
        ''' <param name="icoFilePath">Specified the fully qualified name of the
        ''' icon file.</param>
        ''' <returns>The <see cref="System.Windows.Media.Imaging.BitmapSource"/>
        ''' derived from the icon file.</returns>
        ''' <exception cref="System.ArgumentException">When <paramref
        ''' name="icoFilePath"/> is Null or Empty.</exception>
        Public Shared Function ConvertIcoToBitmapSource(
            icoFilePath As System.String) _
            As System.Windows.Media.Imaging.BitmapSource

            ' Argument checking.
            If (System.String.IsNullOrEmpty(icoFilePath)) Then
                Throw New System.ArgumentException(
                    "ICO file path cannot be null or empty.",
                    NameOf(icoFilePath))
            End If

            Using IcoStream As New System.IO.FileStream(
                icoFilePath, FileMode.Open, FileAccess.Read)

                Dim Decoder As System.Windows.Media.Imaging.BitmapDecoder =
                    System.Windows.Media.Imaging.BitmapDecoder.Create(IcoStream,
                        System.Windows.Media.Imaging.BitmapCreateOptions.None,
                        System.Windows.Media.Imaging.BitmapCacheOption.OnLoad)
                Return Decoder.Frames(0)
            End Using
        End Function ' ConvertIcoToBitmapSource

    End Class ' IcoToBitmapSourceConverter

    ''' <summary>
    ''' DEV: This is not necessarily part of the model. It is a utility for use
    ''' with the sample dialog window. It can be used to load an icon from a
    ''' file at run time.
    ''' </summary>
    ''' <exception cref="System.ArgumentException">When <paramref
    ''' name="icoFilePath"/> is Null or Empty.</exception>
    Private Shared Function GetIconFromFile(
        ByVal icoFilePath As System.String) _
        As System.Windows.Media.ImageSource

        ' This sequence works, but it needs to look for the file in its original
        ' location.
        Dim BSource As System.Windows.Media.Imaging.BitmapSource =
            DialogHost.IcoToBitmapSourceConverter.ConvertIcoToBitmapSource(
                icoFilePath)
        Return BSource
    End Function ' GetIconFromFile

    ''' <summary>
    ''' DEV: This is not necessarily part of the model. It is a utility to
    ''' construct a Pack URI, to load an icon embedded in a DLL, in proper form.
    ''' </summary>
    ''' <param name="referencedAssembly">Specifies the assembly in which the
    ''' icon resource is located.</param>
    ''' <param name="fileName">Specifies the name of the icon file.</param>
    ''' <returns>The constructed string.</returns>
    Private Shared Function GetIconPackURI(
        ByVal referencedAssembly As System.String,
        ByVal fileName As System.String) As System.String

        ' Ref: Referenced Assembly Resource File
        ' https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf#referenced-assembly-resource-file

        Return $"pack://application:,,,/{referencedAssembly}" &
            $";component/Resources/{fileName}"

    End Function ' GetIconPackURI

    ''' <summary>
    ''' DEV: This is not necessarily part of the model. It is a utility for an
    ''' alternate method to select the icon for the dialog. It can be used to
    ''' load an icon embedded in a DLL.
    ''' </summary>
    Private Shared Function GetIconFromResource(ByVal iconPath As System.String) As _
        System.Windows.Media.ImageSource

        ' REF: Setting WPF image source in code
        ' https://stackoverflow.com/questions/350027/setting-wpf-image-source-in-code
        ' REF: Pack URIs in WPF
        ' https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8&redirectedfrom=MSDN

        Dim IconBitmapImage As New System.Windows.Media.Imaging.BitmapImage(
            New System.Uri(iconPath))
        Return IconBitmapImage

    End Function ' GetIconFromResource

#End Region ' "Constructor helpers"

#Region "Constructors"

    ''' <summary>
    ''' Initializes a new instance of the
    ''' <see cref="OSNW.Dialog.DialogHost"/>
    ''' class.
    ''' </summary>
    Public Sub New()
        ' Assign initial defaults.
        With Me

            '            .m_DialogResult = Nothing ' Matches default.
            '            .m_Owner = Nothing ' Matches default.
            .m_ResizeMode = ResizeMode.CanResize ' Matches default.
            '            .m_ShowInTaskbar = False ' Matches default.
            .m_Title = "SET TITLE!"
            '            .m_WindowStartupLocation =
            '                WindowStartupLocation.Manual ' Matches default.

            ' DEV: The HostedDialogWindow is configured with a default icon that
            ' is set in its XAML layout. If m_Icon for the DialogHost is left at
            ' the default Nothing/Null, the XAML entry will be left in place.
            ' m_Icon can be set here and it will override the XAML setting.
            ' The consuming assembly can override Icon after New() and it
            ' will override the setting in New().
            ' Any non-Nothing/Null will be passed to the HostedDialogWindow at
            ' display time.

            ' Programmatic ways to load an icon.

            ' REF: Setting WPF image source in code
            ' https://stackoverflow.com/questions/350027/setting-wpf-image-source-in-code
            ' REF: Pack URIs in WPF
            ' https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8&redirectedfrom=MSDN

            '' DEV: Load an icon from a file.
            '' This sequence works, but it needs to find the file in its
            '' specified (fixed or calculated) location.
            '' "Build Action" can be left as "Resource" and "Copy to Output
            '' Directory" can be left as "Do not copy". It does not need to be
            '' in a Resources folder.
            'Try
            '    Dim ReposPath As System.String = "C:\Users\UserX\source\repos"
            '    Dim ProjectPath As System.String =
            '        "\OSNW-WPF-Custom-Dialog-Models\Models"
            '    Dim FilePath As System.String = "\Resources\InitFromFile.ico"
            '    Dim IconPath As System.String =
            '        $"{ReposPath}{ProjectPath}{FilePath}"
            '    .m_Icon = GetIconFromFile(IconPath)
            'Catch CaughtEx As System.Exception

            '    ' Respond to an anticipated exception.
            '    Dim CaughtBy As System.Reflection.MethodBase =
            '        System.Reflection.MethodBase.GetCurrentMethod
            '    Me.ShowExceptionMessageBox(CaughtBy, CaughtEx)

            '    ' Optional rethrow of the caught exception.
            '    'Throw

            'End Try

            '' DEV: Load an icon from an embedded resource.
            '' Set "Build Action" to "Resource" and "Copy to Output Directory"
            '' to "Do not copy".
            'Try
            '    Dim ReferencedAssembly As System.String = "Models"
            '    Dim EmbeddedFileName As System.String = "InitEmbeddedResource.ico"
            '    Dim IconPath As System.String =
            '        GetIconPackURI(ReferencedAssembly, EmbeddedFileName)
            '    .m_Icon = GetIconFromResource(IconPath)
            'Catch CaughtEx As System.Exception

            '    ' Respond to an anticipated exception.
            '    Dim CaughtBy As System.Reflection.MethodBase =
            '        System.Reflection.MethodBase.GetCurrentMethod
            '    Me.ShowExceptionMessageBox(CaughtBy, CaughtEx)

            '    ' Optional rethrow of the caught exception.
            '    'Throw

            'End Try

            '' DEV: Load an icon from an overridable file.
            'Try

            '    ' REF: C# Executable Executing directory
            '    ' https://stackoverflow.com/questions/7025269/c-sharp-executable-executing-directory
            '    Dim AssyPath As System.String = System.IO.Path.GetDirectoryName(
            '        Assembly.GetExecutingAssembly().Location)

            '    ' Now use that info.
            '    Dim FilePath As System.String = "\Resources\Override.ico"
            '    Dim IconPath As System.String = $"{AssyPath}{FilePath}"
            '    .m_Icon = GetIconFromFile(IconPath)

            'Catch CaughtEx As System.Exception

            '    ' Respond to an anticipated exception.
            '    Dim CaughtBy As System.Reflection.MethodBase =
            '        System.Reflection.MethodBase.GetCurrentMethod
            '    Me.ShowExceptionMessageBox(CaughtBy, CaughtEx)

            '    ' Optional rethrow of the caught exception.
            '    'Throw

            'End Try

        End With
    End Sub ' New

#End Region ' "Constructors"

#Region "Methods"

    ''' <summary>
    ''' Opens a window and returns only when the newly opened window is closed.
    ''' </summary>
    ''' <returns>
    ''' A System.Nullable`1 value of type System.Boolean that specifies whether
    ''' the activity was accepted (true) or canceled (false). The return value
    ''' is the value of the <see cref="System.Windows.Window.DialogResult"/>
    ''' property before a window closes.
    ''' </returns>
    ''' <exception cref="System.InvalidOperationException">
    ''' <see cref="System.Windows.Window.ShowDialog"/> is called on a window
    ''' that is closing (System.Windows.Window.Closing) or has been closed
    ''' (System.Windows.Window.Closed).
    ''' </exception>
    Public Function ShowDialog() As System.Boolean?
        Dim DlgResult As System.Boolean?
        Dim HostedWindow As New OSNW.Dialog.HostedWindow
        Try

            ' Set the properties that get sent to the window.

            HostedWindow.Owner = Me.Owner
            HostedWindow.ResizeMode = Me.ResizeMode
            HostedWindow.ShowInTaskbar = Me.ShowInTaskbar
            HostedWindow.Title = Me.Title
            HostedWindow.WindowStartupLocation = Me.WindowStartupLocation

            ' Only push .Icon if it has been set in the DialogHost.
            If Me.Icon IsNot Nothing Then
                HostedWindow.Icon = Me.Icon
            End If

            HostedWindow.Red = Me.Red
            HostedWindow.Green = Me.Green
            HostedWindow.Blue = Me.Blue
            HostedWindow.TheString = Me.TheString
            HostedWindow.TheInteger = Me.TheInteger

            ' Show the dialog window. Process the result.
            DlgResult = HostedWindow.ShowDialog
            If DlgResult Then
                ' Extract any data being returned.
                Me.Red = HostedWindow.Red
                Me.Green = HostedWindow.Green
                Me.Blue = HostedWindow.Blue
                Me.TheString = HostedWindow.TheString
                Me.TheInteger = HostedWindow.TheInteger
                'Else
                '' Is anything needed when ShowDialog is false?
            End If

        Finally
            ' DISPOSE OF THE WINDOW?????? SET TO NOTHING ENOUGH TO GET RID OF AT
            ' LEAST THE WINDOW????? JUST LET THE WINDOW GO OUT OF SCOPE?????
            ' SETTING TO NOTHING WARNS "Unnecessary assignment of a value to
            ' 'HostedWindow'"
            '            HostedWindow = Nothing
        End Try
        Return DlgResult
    End Function ' ShowDialog

#End Region ' "Methods"

End Class ' DialogHost

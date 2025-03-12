Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Windows
'Imports System.Windows.Controls

' NOTE: <UseWPF>true</UseWPF> may need to be added to the dialogs'
' <projectname>.vbproj file.
'   https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props-desktop
'   Maybe just include PresentationFramework.dll? Or System.Windows?

' For Dialog.ico, set "Build Action" to "Resource" and "Copy to Output
' Directory" to "Do not copy".

' Trying to put XML comments here results in "BC42314 XML comment cannot be
'   applied more than once on a partial class. XML comments for this class will
'   be ignored."
'
'''' <summary>
'''' Represents a model for the window displayed by a <see cref="DialogHost"/>.
'''' </summary>
'''' <remarks>
'''' A <see cref="DialogHost"/> creates a layer of abstraction between its
'''' underlying <c>HostedDialogWindow</c> and the consuming assembly.
'''' <c>HostedDialogWindow</c> is designated as <c>Friend</c> and its XAML
'''' contains <c>x:ClassModifier="Friend"</c>; it is only directly available to
'''' the associated <see cref="DialogHost"/>. Public members of
'''' <see cref="System.Windows.Window"/> are not reachable by the consuming
'''' assembly unless exposed by the <see cref="DialogHost"/>.
'''' </remarks>
Friend Class HostedWindow

    ' These links are from looking into being able to have the dialog window not
    ' be accessible outside of the DLL.

    ' REF: How do I mark a control as 'Private' in WPF?
    ' https://stackoverflow.com/questions/29525968/how-do-i-mark-a-control-as-private-in-wpf

    ' REF: In WPF, how do I make my controls inside a usercontrol private?
    ' https://www.ansaurus.com/question/300255-in-wpf-how-do-i-make-my-controls-inside-a-usercontrol-private

    ' REF: x:FieldModifier Directive
    ' https://learn.microsoft.com/en-us/dotnet/desktop/xaml-services/xfieldmodifier-directive

    ' REF: x:ClassModifier Directive
    ' https://learn.microsoft.com/en-us/dotnet/desktop/xaml-services/xclassmodifier-directive
    ' For Microsoft Visual Basic .NET, the string to pass to designate TypeAttributes.NotPublic is Friend.
    ' That is done in HostedDialogWindow.xaml.

    ' A signal to distinguish between aborts and acceptance, at closure.
    Private ClosingViaOk As System.Boolean

    ' A signal to prevent recursive responses.
    Private SettingSliders As System.Boolean

#Region "Properties"

    ' DEV: These specific properties are not intended as part of the model. They
    ' are included to support operation of the example. In general, properties
    ' like these should not need examination by the setter; that should normally
    ' be handled in the associated <see cref="DialogHost"/>.
    Public Property Red As System.Byte
    Public Property Green As System.Byte
    Public Property Blue As System.Byte
    Public Property TheString As System.String
    Public Property TheInteger As System.Int32

#End Region ' "Properties"

#Region "Internal utilities"
    ' DEV: These utilities are not intended as part of the model. Any dialog may
    ' need to perform operations unique to itself.

    ''' <summary>
    ''' DEV: Copied from ColorUtils.vb. This is not part of the model. It is a
    ''' utility for use with the sample dialog window. It returns a foreground
    ''' color for ColorTextBox that is readable against the background color.
    ''' </summary>
    Private Shared Function ContrastingBW(ByVal r As System.Byte,
        ByVal g As System.Byte, ByVal b As System.Byte) _
        As System.Windows.Media.Color

        Return If(
            System.Math.Sqrt((255 - r) ^ 2 + (255 - g) ^ 2 + (255 - b) ^ 2) >
                System.Math.Sqrt(r ^ 2 + g ^ 2 + b ^ 2),
            System.Windows.Media.Colors.White,
            System.Windows.Media.Colors.Black)
    End Function ' ContrastingBW

#End Region ' "Internal utilities"

#Region "Model utilities"
    ' These utilities are intended as part of the model, but the implementation
    ' may vary or they may be omitted in individual cases.

    ''' <summary>
    ''' Update visual items that reflect the impact of state changes.
    ''' </summary>
    Private Sub UpdateVisuals()
        ' DEV: The entries below are speficic the the sample dialog window.
        Dim BackgroundColor As System.Windows.Media.Color =
            System.Windows.Media.Color.FromRgb(Me.Red, Me.Green, Me.Blue)
        Me.ColorTextBox.Background =
            New System.Windows.Media.SolidColorBrush(BackgroundColor)
        Dim ForegroundColor As System.Windows.Media.Color =
            ContrastingBW(Me.Red, Me.Green, Me.Blue)
        Me.ColorTextBox.Foreground =
            New System.Windows.Media.SolidColorBrush(ForegroundColor)
        Me.ColorTextBox.Text = $"R:{Me.Red} G:{Me.Green} B:{Me.Blue}"
    End Sub ' UpdateVisuals

    ''' <summary>
    ''' Evaluate whether there is any reason to consider aborting closure via
    ''' <c>CancelButton</c>, etc.
    ''' </summary>
    ''' <returns><c>True</c> if closure via <c>CancelButton</c>, etc. should be
    ''' reconsidered; otherwise, <c>False</c>.</returns>
    Private Function WarnClose() As System.Boolean
        ' DEV: Add code here to determine if some risky condition exists when
        ' faced with a closure. If so, display a message to decide how to
        ' proceed. This can be left as is and returning False. It can also be
        ' deleted, or commented out, to avoid the useless call.
        Return False
    End Function

    ''' <summary>
    ''' Evaluate whether there is any reason to prevent closure.
    ''' </summary>
    ''' <returns><c>True</c> if closure via <c>CancelButton</c>, etc. should be
    ''' prevented; otherwise, <c>False</c>.</returns>
    Private Function BlockClose() As System.Boolean
        ' DEV: Add code here to determine if closure should be prevented. If so,
        ' display a message or other visual indication to explain the problem.
        ' This can be left as is and returning False. It can also be deleted, or
        ' commented out, to avoid the useless call.
        Return False
    End Function ' BlockClose

    ''' <summary>
    ''' Evaluate whether everything is ready to allow closure via
    ''' <c>OkButton</c>.
    ''' </summary>
    ''' <returns><c>True</c> if everything is ready to allow closure via
    ''' OkButton; otherwise, <c>False</c>.</returns>
    Private Function OkToOk() As System.Boolean

        ' DEV: The specific code here is unique to the sample dialog. The
        ' underlying reason for the function may be of use in certain cases.
        ' Add code here to determine if closure is ok. If not, display a message
        ' or other visual indication to explain the problem. This can be similar
        ' to below. It can also be deleted, or commented out, to avoid a useless
        ' call.

        ' Does IntegerTextBox contain a valid integer string?
        Dim TestDestination As System.Int32
        If System.Int32.TryParse(Me.IntegerTextBox.Text,
                                 TestDestination) Then
            Return True
        Else
            ' Display a message?
            Return False
        End If
    End Function ' OkToOk

#End Region ' "Model utilities"

#Region "Event Implementations"
    ' These routines contain detailed implementations of Event handlers.

    Private Sub DoWindow_Loaded(sender As Object, e As RoutedEventArgs)

        ' Update visual items based on the incoming state.
        With Me

            ' DEV: The specific code here is unique to the sample dialog. The
            ' underlying reason for the Sub may be of use in certain cases.

            ' Suppress having Red changed when SliderR moves to match Red.
            .SettingSliders = True
            Try
                .SliderR.Value = .Red
                .SliderG.Value = .Green
                .SliderB.Value = .Blue
            Finally
                ' Restore normal slider response.
                .SettingSliders = False
            End Try

            .UpdateVisuals()
            .StringTextBox.Text = .TheString
            .IntegerTextBox.Text = .TheInteger.ToString

        End With
    End Sub ' DoWindow_Loaded

#End Region ' "Event Implementations"

#Region "Model Events"

    ''' <summary>
    ''' Occurs when this <c>Window</c> is initialized. Backing fields and local
    ''' variables can usually be set after arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Initialized"/>.
    ''' </summary>
    Private Sub Window_Initialized(sender As Object, e As EventArgs) _
        Handles Me.Initialized

        Me.ClosingViaOk = False
    End Sub ' Window_Initialized

    ''' <summary>
    ''' Occurs when the <c>Window</c> is laid out, rendered, and ready for
    ''' interaction. Sometimes updates have to wait until arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Loaded"/>.
    ''' </summary>
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) _
        Handles Me.Loaded

        Me.DoWindow_Loaded(sender, e)
    End Sub ' Window_Loaded

    ''' <summary>
    ''' Occurs directly after <see cref="System.Windows.Window.Close"/> is
    ''' called, and can be handled to cancel window closure. See
    ''' <see cref="System.Windows.Window.Closing"/>.
    ''' </summary>
    ''' <remarks>
    ''' This gets hit for <c>CancelButton</c>, Escape, ALT+F4,
    ''' System menu | Close, and the window's red X. It also gets hit whenever
    ''' <c>DialogResult</c> is set. It also gets hit for <c>OkButton</c>, if
    ''' only because it sets <c>DialogResult</c>.
    ''' </remarks>
    Private Sub Window_Closing(sender As Object,
        e As ComponentModel.CancelEventArgs) _
        Handles Me.Closing

        ' In general, do not interfere when OkButton was used.
        If Me.ClosingViaOk Then
            Exit Sub ' Early exit.
        End If

        '' This is an option for an absolute rejection.
        '' Do a local evaluation, or implement and call BlockClose(),
        '' to determine if the closure should be ignored for some reason.
        'If BlockClose() Then
        '    e.Cancel = True
        '    Exit Sub ' Early exit.
        'End If

        '' This is an option to make a choice.
        '' Do a local evaluation, or implement and call WarnClose(), to
        '' determine if the closure should be reconsidered for some reason.
        '' REF: https://learn.microsoft.com/en-us/dotnet/api/system.windows.window.closing?view=windowsdesktop-9.0#system-windows-window-closing
        'If Me.WarnClose() Then
        '    Dim Msg As System.String = "Allow close?"
        '    Dim MsgResult As System.Windows.MessageBoxResult =
        '        System.Windows.MessageBox.Show(Msg, "Approve closure",
        '            System.Windows.MessageBoxButton.YesNo,
        '            System.Windows.MessageBoxImage.Warning)
        '    If MsgResult = MessageBoxResult.No Then
        '        ' If user doesn't want to close, cancel closure.
        '        e.Cancel = True
        '        Exit Sub ' Early exit.
        '    End If
        'End If

        ' Falling through to here allows the closure to continue.

    End Sub ' Window_Closing

    '''' <summary>
    '''' Occurs when the window is about to close. See
    '''' <see cref="System.Windows.Window.Closed"/>.
    '''' </summary>
    '''' <remarks>Once this event is raised, a window cannot be prevented from
    '''' closing.</remarks>
    'Private Sub Window_Closed(sender As Object, e As EventArgs) _
    '    Handles Me.Closed

    '    '
    '    Throw New NotImplementedException()
    '    '
    'End Sub ' Window_Closed

    '''' <summary>
    '''' Abandon the current dialog session.
    '''' </summary>
    '''' <remarks>
    '''' This only responds to <c>CancelButton</c> or Escape; it does not
    '''' respond to ALT+F4, System menu | Close, or the window's red X. See
    '''' <see cref="Window_Closing"/>.
    '''' </remarks>
    'Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) _
    '    Handles CancelButton.Click

    '    '
    '    Throw New NotImplementedException()
    '    '
    'End Sub ' CancelButton_Click

    ''' <summary>
    ''' Fill in any updates to the passed data then close the window.
    ''' </summary>
    Private Sub OkButton_Click(sender As Object, e As Windows.RoutedEventArgs) _
        Handles OkButton.Click

        ' Do a local evaluation, or implement and call OkToOk(), to determine
        ' if the current status is suitable for closure.
        If OkToOk() Then

            ' Set the return values.
            Me.TheString = Me.StringTextBox.Text
            Me.TheInteger = Int32.Parse(Me.IntegerTextBox.Text)

            Me.ClosingViaOk = True
            Me.DialogResult = True

            'Else
            ' Display a message?
            ' Ignore the click and wait for Cancel or correction.
        End If
    End Sub ' OkButton_Click

#End Region ' "Model Events"

#Region "Example Events"
    ' DEV: These events are not intended as part of the model. They are included
    ' to support operation of the example.

    Private Sub SliderR_ValueChanged(sender As Object,
        e As RoutedPropertyChangedEventArgs(Of System.Double)) _
        Handles SliderR.ValueChanged

        If Not Me.SettingSliders Then
            Me.Red = CType(SliderR.Value, System.Byte)
            Me.UpdateVisuals()
        End If
    End Sub ' SliderR_ValueChanged

    Private Sub SliderG_ValueChanged(sender As Object,
        e As RoutedPropertyChangedEventArgs(Of System.Double)) _
        Handles SliderG.ValueChanged

        If Not Me.SettingSliders Then
            Me.Green = CType(SliderG.Value, System.Byte)
            Me.UpdateVisuals()
        End If
    End Sub ' SliderG_ValueChanged

    Private Sub SliderB_ValueChanged(sender As Object,
        e As RoutedPropertyChangedEventArgs(Of System.Double)) _
        Handles SliderB.ValueChanged

        If Not Me.SettingSliders Then
            Me.Blue = CType(SliderB.Value, System.Byte)
            Me.UpdateVisuals()
        End If
    End Sub ' SliderB_ValueChanged

#End Region ' "Example Events"

End Class ' HostedWindow

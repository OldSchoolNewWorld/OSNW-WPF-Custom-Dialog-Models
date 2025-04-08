Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports SHWV = OSNW.Dialog.HostedWindow.SharedHostedWindowValues


' For Dialog.ico, set "Build Action" to "Resource" and "Copy to Output
' Directory" to "Do not copy".

' Trying to put XML comments here results in "BC42314 XML comment cannot be
'   applied more than once on a partial class. XML comments for this class will
'   be ignored."
' The XML comments here conflict with the auto-generated entry in
'   Consumer\obj\Debug\net8.0-windows8.0\EmbeddedWindow.g.vb"
'   Deleting those lines made the warning go away, but the lines came back on
'   the next rebuild and the warning showed again.
'
'''' <summary>
'''' Represents a model for a dialog window embedded in the consuming
'''' application.
'''' </summary>
'''' <remarks>
'''' <para>
'''' This class is created as part of the consuming application, not as a
'''' reusable dialog available from a DLL. It is dedicated to use by the
'''' consuming assembly.
'''' </para>
'''' </remarks>
Public Class EmbeddedWindow

    ' A signal to distinguish between aborts and acceptance at closure.
    Private ClosingViaOk As System.Boolean

    ' DEV: This specific value is not intended as part of the model. It is
    ' included to support operation of the example.
    ' Prevent looping responses.
    Private SettingSliders As System.Boolean

#Region "Properties"

    ' DEV: These specific properties are not intended as part of the model. They
    ' are included to support operation of the example.
    Public Property Red As System.Byte
    Public Property Green As System.Byte
    Public Property Blue As System.Byte
    Public Property TheString As System.String
    Public Property TheInteger As System.Int32

#End Region ' "Properties"

#Region "Internal Utilities"
    ' DEV: These utilities are not necessarily intended as part of the model.

    '''' <summary>
    '''' Copied from ColorUtils.vb.
    '''' DEV: This is not actually part of the model. It is a utility for use
    '''' with the sample dialog window. It makes the foreground text in
    '''' ColorTextBox readable against the background color.
    '''' </summary>
    Private Shared Function ContrastingBW(ByVal r As System.Byte,
        ByVal g As System.Byte, ByVal b As System.Byte) _
        As System.Windows.Media.Color

        Return If(
            System.Math.Sqrt((255 - r) ^ 2 + (255 - g) ^ 2 + (255 - b) ^ 2) >
                System.Math.Sqrt(r ^ 2 + g ^ 2 + b ^ 2),
            System.Windows.Media.Colors.White,
            System.Windows.Media.Colors.Black)
    End Function ' ContrastingBW

    '''' <summary>
    '''' DEV: This is not, necessarily, part of the model. It is a utility for
    '''' use with the sample dialog window. It provides visual feedback of the
    '''' up/down RGB selections. It does illustrate that similar activities may
    '''' be required to reflect the impact of state changes.
    '''' </summary>
    Private Sub UpdateColorTextBox()
        Dim BackgroundColor As System.Windows.Media.Color =
            System.Windows.Media.Color.FromRgb(Me.Red, Me.Green, Me.Blue)
        Me.ColorTextBox.Background =
            New System.Windows.Media.SolidColorBrush(BackgroundColor)
        Dim ForegroundColor As System.Windows.Media.Color =
            ContrastingBW(Me.Red, Me.Green, Me.Blue)
        Me.ColorTextBox.Foreground =
            New System.Windows.Media.SolidColorBrush(ForegroundColor)
        Me.ColorTextBox.Text = $"R:{Me.Red} G:{Me.Green} B:{Me.Blue}"
    End Sub ' UpdateColorTextBox

#End Region ' "Internal Utilities"

#Region "Dialog Model Utilities"
    ' DEV: These utilities are intended as part of the model.

    '''' <summary>
    '''' Evaluate whether there is any reason to consider aborting closure via
    '''' <c>CancelButton</c>, etc.
    '''' </summary>
    '''' <returns><c>True</c> if closure via <c>CancelButton</c>, etc. should be
    '''' reconsidered; otherwise, <c>False</c>.</returns>
    'Private Function WarnClose() As System.Boolean
    '    Return False
    'End Function

    '''' <summary>
    '''' Evaluate whether there is any reason to prevent closure.
    '''' </summary>
    '''' <returns><c>True</c> if closure via <c>CancelButton</c>, etc. should be
    '''' prevented; otherwise, <c>False</c>.</returns>
    'Private Function BlockClose() As System.Boolean
    '    Return False
    'End Function ' BlockClose

    ''' <summary>
    ''' Evaluate whether everything is ready to allow closure via OkButton.
    ''' </summary>
    ''' <returns><c>True</c> if everything is ready to allow closure via
    ''' OkButton; otherwise, <c>False</c>.</returns>
    Private Function OkToOk() As System.Boolean

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

#End Region ' "Dialog Model Utilities"

#Region "Dialog Model Events"
    ' DEV: These events are intended as part of the model.

    ''' <summary>
    ''' Occurs when this <c>Window</c> is initialized. Backing fields and local
    ''' variables can usually be set after arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Initialized"/>.
    ''' </summary>
    Private Sub Window_Initialized(sender As Object, e As EventArgs) _
        Handles Me.Initialized

        With Me

            ' HostedWindow.SharedHostedWindowValues contains the shared
            ' initialization definitions.
            ' HostedWindow.New, DialogHost.New,
            ' DialogWindow.Window_Initialized, and
            ' EmbeddedWindow.Window_Initialized, should reference the
            ' shared values so that changes in the definitions will
            ' be matched by the windows.

            ' Window items.
            .ResizeMode = SHWV.DEFAULTRESIZEMODE
            .ShowInTaskbar = SHWV.DEFAULTSHOWINTASKBAR
            .Title = SHWV.DEFAULTDIALOGTITLE
            .WindowStartupLocation = SHWV.DEFAULTWINDOWSTARTUPLOCATION

        End With

        Me.ClosingViaOk = False

    End Sub ' Window_Initialized

    ''' <summary>
    ''' Occurs when the <c>Window</c> is laid out, rendered, and ready for
    ''' interaction. Sometimes updates have to wait until arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Loaded"/>.
    ''' </summary>
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) _
        Handles Me.Loaded

        ' Update visual items based on the incoming state.
        With Me

            .SettingSliders = True
            Try
                .SliderR.Value = .Red
                .SliderG.Value = .Green
                .SliderB.Value = .Blue
            Finally
                .SettingSliders = False
            End Try

            .UpdateColorTextBox()
            .StringTextBox.Text = .TheString
            .IntegerTextBox.Text = .TheInteger.ToString

        End With
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
        '' Do a local evaluation, or implement and call WarnClose(), to determine
        '' if the closure should be reconsidered for some reason.
        '' REF: https://learn.microsoft.com/en-us/dotnet/api/system.windows.window.closing?view=windowsdesktop-9.0#system-windows-window-closing
        'If Me.WarnClose() Then
        '    Dim Msg As System.String = "Allow close?"
        '    Dim MsgResult As System.Windows.MessageBoxResult =
        '        System.Windows.MessageBox.Show(Msg, "Approve closure",
        '            System.Windows.MessageBoxButton.YesNo,
        '            System.Windows.MessageBoxImage.Warning)
        '    If MsgResult = System.Windows.MessageBoxResult.No Then
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
    '''' <see cref="TheHostedWindow_Closing"/>.
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

#End Region ' "Dialog Model Events"

#Region "Example Events"
    ' DEV: These events are not intended as part of the model. They are included
    ' to support operation of the example.

    Private Sub SliderR_ValueChanged(sender As Object,
        e As RoutedPropertyChangedEventArgs(Of System.Double)) _
        Handles SliderR.ValueChanged

        If Not Me.SettingSliders Then
            Me.Red = CType(SliderR.Value, System.Byte)
            Me.UpdateColorTextBox()
        End If
    End Sub ' SliderR_ValueChanged

    Private Sub SliderG_ValueChanged(sender As Object,
        e As RoutedPropertyChangedEventArgs(Of System.Double)) _
        Handles SliderG.ValueChanged

        If Not Me.SettingSliders Then
            Me.Green = CType(SliderG.Value, System.Byte)
            Me.UpdateColorTextBox()
        End If
    End Sub ' SliderG_ValueChanged

    Private Sub SliderB_ValueChanged(sender As Object,
        e As RoutedPropertyChangedEventArgs(Of System.Double)) _
        Handles SliderB.ValueChanged

        If Not Me.SettingSliders Then
            Me.Blue = CType(SliderB.Value, System.Byte)
            Me.UpdateColorTextBox()
        End If
    End Sub ' SliderB_ValueChanged

#End Region ' "Example Events"

End Class ' EmbeddedWindow

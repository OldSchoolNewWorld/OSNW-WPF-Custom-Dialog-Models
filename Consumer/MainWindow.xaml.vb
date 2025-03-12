Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Reflection



' TODOs
' What needs to be done about disposing of a dialog after it has been used?
'   Does it just go out of scope and have no references?
' Is it possible to get XML comments to work on the windows?
Class MainWindow

    ' DEV: These specific values are not intended as part of the model. They are
    ' included to support operation of the example.
    Private Red As System.Byte
    Private Green As System.Byte
    Private Blue As System.Byte

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

    ''' <summary>
    ''' DEV: This is not, necessarily, part of the model. It is a utility for
    ''' use with the sample dialog window. It makes the foreground text in
    ''' ColorTextBox readable against the background color. It does illustrate
    ''' that similar activities may be required to reflect the impact of state
    ''' changes.
    ''' </summary>
    Private Sub UpdateColorTextBox()
        Dim BackgroundColor As System.Windows.Media.Color =
            System.Windows.Media.Color.FromRgb(Me.Red, Me.Green, Me.Blue)
        Me.ColorTextBox.Background =
            New System.Windows.Media.SolidColorBrush(BackgroundColor)
        Dim ForegroundColor As System.Windows.Media.Color =
            ContrastingBW(Me.Red, Me.Green, Me.Blue)
        Me.ColorTextBox.Foreground =
            New System.Windows.Media.SolidColorBrush(ForegroundColor)
        Me.ColorTextBox.Text = "The color from the dialog - " &
            $"R:{Me.Red} G:{Me.Green} B:{Me.Blue}"
    End Sub ' UpdateColorTextBox

#Region "Model Events"
    ' DEV: These events are intended as part of the model.

    ''' <summary>
    ''' Occurs when this <c>Window</c> is initialized. Backing fields can
    ''' usually be set after arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Initialized"/>.
    ''' </summary>
    Private Sub Window_Initialized(sender As Object, e As EventArgs) _
        Handles Me.Initialized

        With Me
            .Red = 64
            .Green = 128
            .Blue = 192
        End With
    End Sub ' Window_Initialized

    ''' <summary>
    ''' Occurs when the <c>Window</c> is laid out, rendered, and ready for
    ''' interaction. Sometimes updates have to wait until arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Loaded"/>.
    ''' </summary>
    Private Sub Window_Loaded(
        sender As Object, e As RoutedEventArgs) _
        Handles Me.Loaded

        Me.UpdateColorTextBox()
    End Sub ' Window_Loaded

    '''' <summary>
    '''' Occurs directly after System.Windows.Window.Close is called, and can be
    '''' handled to cancel window closure. See
    '''' <see cref="System.Windows.Window.Closing"/>.
    '''' </summary>
    'Private Sub Window_Closing(
    '    sender As Object, e As ComponentModel.CancelEventArgs) _
    '    Handles Me.Closing

    '    '
    '    Throw New NotImplementedException()
    '    '
    'End Sub ' Window_Closing

    '''' <summary>
    '''' Occurs when the window is about to close. See
    '''' <see cref="System.Windows.Window.Closed"/>.
    '''' </summary>
    'Private Sub Window_Closed(sender As Object, e As EventArgs) _
    '    Handles Me.Closed

    '    '
    '    Throw New NotImplementedException()
    '    '
    'End Sub ' Window_Closed

    Private Sub CloseButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles CloseButton.Click

        Me.Close()
    End Sub ' CloseButton_Click

#End Region ' "Model Events"

#Region "Example Events"
    ' DEV: These events are not intended as part of the model. They are included
    ' to support operation of the example.

    ''' <summary>
    ''' Displays a dialog window embedded in the consuming assembly and
    ''' processes the result.
    ''' </summary>
    Private Sub EmbeddedDialogWindowButton_Click(
        sender As Object, e As RoutedEventArgs) _
        Handles EmbeddedDialogWindowButton.Click

        ' This can be used to allow the caller to specify a contextual
        ' title, perhaps to include a file name.
        Const WINDOWTITLE As System.String = "Embedded Dialog Window"

        ' Set up the data to be passed to the dialog.
        Dim StringAsInteger As System.Int32 =
            System.Int32.Parse(CType(ShowIntegerLabel.Content, System.String))

        ' Set up the dialog.
        Dim Dlg As New EmbeddedWindow With {
            .Owner = Me,
            .ShowInTaskbar = False,
            .Title = WINDOWTITLE,
            .WindowStartupLocation =
                System.Windows.WindowStartupLocation.CenterScreen,
            .Red = Me.Red,
            .Green = Me.Green,
            .Blue = Me.Blue,
            .TheInteger = StringAsInteger,
            .TheString = CType(Me.ShowStringLabel.Content, System.String)}

        ' DEV: If desired, change .Icon. A default icon is set in the XAML
        ' layout. It can be changed either in the XAML or by an assignment
        ' here. One case for that would be if the application icon is to be
        ' sent to the dialog.
        'Dlg.Icon = Nothing

        Dim DlgResult As System.Boolean? = Dlg.ShowDialog()
        If DlgResult Then

            ' Extract any data being returned.
            Me.Red = Dlg.Red
            Me.Green = Dlg.Green
            Me.Blue = Dlg.Blue

            ' Update the visuals.
            Me.UpdateColorTextBox()
            Me.ShowStringLabel.Content = Dlg.TheString
            Me.ShowIntegerLabel.Content = Dlg.TheInteger

            'Else
            '' Is anything needed when ShowDialog is false?
        End If
    End Sub ' EmbeddedDialogWindowButton_Click

    ''' <summary>
    ''' Displays a reusable dialog window embedded in a DLL and processes the
    ''' result. <see cref="OSNW.Dialog.DialogWindow"/> exposes accessible
    ''' features of <see cref="Window"/>, whereas
    ''' <see cref="OSNW.Dialog.DialogHost"/> exposes only limited features.
    ''' </summary>
    Private Sub DllDialogWindowButton_Click(
        sender As Object, e As RoutedEventArgs) _
        Handles DllDialogWindowButton.Click

        Const WINDOWTITLE As System.String = "Dialog as a Window in a DLL"

        ' Set up the data to be passed to the dialog.
        Dim StringAsInteger As System.Int32 =
            System.Int32.Parse(CType(ShowIntegerLabel.Content, System.String))

        ' Set up the dialog.
        Dim Dlg As New OSNW.Dialog.DialogWindow With {
            .Owner = Me,
            .ShowInTaskbar = False,
            .Title = WINDOWTITLE,
            .WindowStartupLocation =
                System.Windows.WindowStartupLocation.CenterScreen,
            .Red = Me.Red,
            .Green = Me.Green,
            .Blue = Me.Blue,
            .TheInteger = StringAsInteger,
            .TheString = CType(Me.ShowStringLabel.Content, System.String)}
        ' DEV: If desired, change .Icon. A default icon is set in the DLL.
        '        Dlg.Icon = Nothing

        ' Show the dialog. Process the result.
        Try
            Dim DlgResult As System.Boolean? = Dlg.ShowDialog
            If DlgResult Then

                ' Extract any data being returned.
                Me.Red = Dlg.Red
                Me.Green = Dlg.Green
                Me.Blue = Dlg.Blue

                ' Update the visuals.
                Me.UpdateColorTextBox()
                Me.ShowStringLabel.Content = Dlg.TheString
                Me.ShowIntegerLabel.Content = Dlg.TheInteger

                'Else
                '' Is anything needed when ShowDialog is false?
            End If
        Finally
            ' DISPOSE OF THE WINDOW?????? SET TO NOTHING ENOUGH TO GET RID OF AT
            ' LEAST THE WINDOW????? JUST LET THE WINDOW GO OUT OF SCOPE?????
            ' SETTING TO NOTHING WARNS "Unnecessary assignment of a value to 'Dlg'"
            '            Dlg = Nothing
        End Try

    End Sub ' DllDialogWindowButton_Click

    ''' <summary>
    ''' Displays a reusable dialog window embedded in a DLL and processes the
    ''' result.
    ''' DEV: <see cref="OSNW.Dialog.DialogWindow"/> exposes accessible
    ''' features of <see cref="System.Windows.Window"/>, whereas
    ''' <see cref="OSNW.Dialog.DialogHost"/> exposes only limited features.
    ''' </summary>
    Private Sub HostedDialogButton_Click(
        sender As Object, e As RoutedEventArgs) _
        Handles HostedDialogButton.Click

        Const WINDOWTITLE As System.String = "Dialog Hosted by a Class"

        ' Set up the data to be passed to the dialog.
        Dim StringAsInteger As System.Int32 =
            System.Int32.Parse(CType(ShowIntegerLabel.Content, System.String))

        ' Set up the dialog.
        ' DEV: Do not set .Icon here?????????
        Dim Dlg As New OSNW.Dialog.DialogHost With {
            .Owner = Me,
            .ShowInTaskbar = False,
            .Title = WINDOWTITLE,
            .WindowStartupLocation =
                System.Windows.WindowStartupLocation.CenterScreen,
            .Red = Me.Red,
            .Green = Me.Green,
            .Blue = Me.Blue,
            .TheInteger = StringAsInteger,
            .TheString = CType(Me.ShowStringLabel.Content, System.String)}

        '' DEV: If desired, change .Icon. A default icon is set in the DLL.
        'Dlg.Icon = Me.Icon

        ' Show the dialog. Process the result.
        Try
            Dim DlgResult As System.Boolean? = Dlg.ShowDialog
            If DlgResult Then

                ' Extract any data being returned.
                Me.Red = Dlg.Red
                Me.Green = Dlg.Green
                Me.Blue = Dlg.Blue

                ' Update the visuals.
                Me.UpdateColorTextBox()
                Me.ShowStringLabel.Content = Dlg.TheString
                Me.ShowIntegerLabel.Content = Dlg.TheInteger

                'Else
                '' Is anything needed when ShowDialog is false?
            End If
        Finally
            ' DISPOSE OF THE WINDOW?????? SET TO NOTHING ENOUGH TO GET RID OF AT
            ' LEAST THE WINDOW????? JUST LET THE WINDOW GO OUT OF SCOPE?????
            ' SETTING TO NOTHING WARNS "Unnecessary assignment of a value to 'Dlg'"
            '            Dlg = Nothing
        End Try

    End Sub ' HostedDialogButton_Click

#End Region ' "Example Events"

End Class ' MainWindow

REM *********************************************************************
REM Input.vb
REM Module code that provides an abstracted interface to the SlimDX
REM DirectInput system.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Public Module Input
    Private SingletonInstance As InstanceClass

    Public ReadOnly Property Singleton() As InstanceClass
        Get
            Return SingletonInstance
        End Get
    End Property

    Public Function Instantiate(RenderForm As SlimDX.Windows.RenderForm) As InstanceClass
        If (SingletonInstance Is Nothing) Then
            SingletonInstance = New InstanceClass(RenderForm)
        End If

        Return SingletonInstance
    End Function

    Public Enum MouseButton
        Right = 1
        Left = 0
        Middle = 3
    End Enum

    Public Enum Key
        A = SlimDX.DirectInput.Key.A
        B = SlimDX.DirectInput.Key.B
        C = SlimDX.DirectInput.Key.C
        D = SlimDX.DirectInput.Key.D
        E = SlimDX.DirectInput.Key.E
        F = SlimDX.DirectInput.Key.F
        G = SlimDX.DirectInput.Key.G
        H = SlimDX.DirectInput.Key.H
        I = SlimDX.DirectInput.Key.I
        J = SlimDX.DirectInput.Key.J
        K = SlimDX.DirectInput.Key.K
        L = SlimDX.DirectInput.Key.L
        M = SlimDX.DirectInput.Key.M
        N = SlimDX.DirectInput.Key.N
        O = SlimDX.DirectInput.Key.O
        P = SlimDX.DirectInput.Key.P
        Q = SlimDX.DirectInput.Key.Q
        R = SlimDX.DirectInput.Key.R
        S = SlimDX.DirectInput.Key.S
        T = SlimDX.DirectInput.Key.T
        U = SlimDX.DirectInput.Key.U
        V = SlimDX.DirectInput.Key.V
        W = SlimDX.DirectInput.Key.W
        X = SlimDX.DirectInput.Key.X
        Y = SlimDX.DirectInput.Key.Y
        Z = SlimDX.DirectInput.Key.Z
        Enter = SlimDX.DirectInput.Key.Return
        Escape = SlimDX.DirectInput.Key.Escape
        NumpadPlus = SlimDX.DirectInput.Key.NumberPadPlus
        NumpadMinus = SlimDX.DirectInput.Key.NumberPadMinus
        ArrowUp = SlimDX.DirectInput.Key.UpArrow
        ArrowDown = SlimDX.DirectInput.Key.DownArrow
        ArrowRight = SlimDX.DirectInput.Key.RightArrow
        ArrowLeft = SlimDX.DirectInput.Key.LeftArrow

        F12 = SlimDX.DirectInput.Key.F12
        F11 = SlimDX.DirectInput.Key.F11
        F10 = SlimDX.DirectInput.Key.F10
        F9 = SlimDX.DirectInput.Key.F9
        F8 = SlimDX.DirectInput.Key.F8
        F7 = SlimDX.DirectInput.Key.F7
        F6 = SlimDX.DirectInput.Key.F6
        F5 = SlimDX.DirectInput.Key.F5
        F4 = SlimDX.DirectInput.Key.F4
        F3 = SlimDX.DirectInput.Key.F3
        F2 = SlimDX.DirectInput.Key.F2
        F1 = SlimDX.DirectInput.Key.F1

        Space = SlimDX.DirectInput.Key.Space

        ShiftLeft = SlimDX.DirectInput.Key.LeftShift
        ShiftRight = SlimDX.DirectInput.Key.RightShift

        CtrlLeft = SlimDX.DirectInput.Key.LeftControl
        CtrlRight = SlimDX.DirectInput.Key.RightControl
    End Enum

    Public Class InstanceClass
        Inherits HellionEngine.Core.EngineObject

        Public KeysPressed(255) As Boolean

        REM Reference to the Active DirectInput Object.
        Private DirectInput As SlimDX.DirectInput.DirectInput = Nothing
        REM Reference to the Keyboard Object.
        Private KeyboardInput As SlimDX.DirectInput.Keyboard = Nothing
        REM Reference to the Mouse Object.
        Private MouseInput As SlimDX.DirectInput.Mouse = Nothing
        REM Reference to the form being used for rendering.
        Private RenderForm As SlimDX.Windows.RenderForm = Nothing

        Public Event KeyPressed(Key As Key)
        Public Event KeyReleased(Key As Key)

        Public Sub New(NewRenderForm As SlimDX.Windows.RenderForm)
            MyBase.New()

            Try
                Me.DirectInput = New SlimDX.DirectInput.DirectInput

                REM Initialize the DirectInput keyboard object.
                Me.KeyboardInput = New SlimDX.DirectInput.Keyboard(DirectInput)
                Me.KeyboardInput.SetCooperativeLevel(NewRenderForm, SlimDX.DirectInput.CooperativeLevel.Nonexclusive Or SlimDX.DirectInput.CooperativeLevel.Background)
                Me.KeyboardInput.Acquire()

                REM Initialize the DirectInput mouse object.
                Me.MouseInput = New SlimDX.DirectInput.Mouse(DirectInput)
                Me.MouseInput.SetCooperativeLevel(NewRenderForm, SlimDX.DirectInput.CooperativeLevel.Nonexclusive Or SlimDX.DirectInput.CooperativeLevel.Background)
                Me.MouseInput.Acquire()
            Catch ex As SlimDX.DirectInput.DirectInputException
                HellionEngine.Logging.Write("Input.vb [InstanceClass+New]: " + ex.Message)
                Return
            End Try

            Me.RenderForm = NewRenderForm
            HellionEngine.Logging.Write("Input.vb [InstanceClass+New]: Initialization Successful.")
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            For Key As Integer = 0 To 255
                Dim KeyPressed As Boolean = Me.KeyboardKeyPressed(Key)

                If (KeyPressed And Not KeysPressed(Key)) Then
                    RaiseEvent KeyPressed(Key)
                ElseIf (Not KeyPressed And KeysPressed(Key)) Then
                    RaiseEvent KeyReleased(Key)
                End If

                KeysPressed(Key) = KeyPressed
            Next
        End Sub

        REM Returns whether or not the requested key is currently pressed.
        Public Function KeyboardKeyPressed(Key As SlimDX.DirectInput.Key) As Boolean
            If (Not (Me.KeyboardInput Is Nothing) And Not Me.KeyboardInput.Disposed) Then
                Dim KeyboardState As SlimDX.DirectInput.KeyboardState = Me.KeyboardInput.GetCurrentState()
                Return KeyboardState.IsPressed(Key)
            End If

            Return False
        End Function

        REM Returns whether or not the requested key is currently not pressed.
        Public Function KeyboardKeyReleased(Key As SlimDX.DirectInput.Key) As Boolean
            Return Not KeyboardKeyPressed(Key)
        End Function

        REM Returns whether or not the specified mouse button is pressed.
        Public Function MouseButtonPressed(Button As Integer)
            Dim MouseState As SlimDX.DirectInput.MouseState = Me.MouseInput.GetCurrentState()
            Return MouseState.IsPressed(Button)
        End Function

        REM Returns whether or not the specified mouse button is NOT pressed.
        Public Function MouseButtonReleased(Button As Integer)
            Return Not MouseButtonPressed(Button)
        End Function

        REM Returns the current mouse X,Y coordinates relative to the top left corner of our render.
        REM Note: Apparently the DirectInput mouse object only measures mouse deltas, that's why the math below exists to get
        REM the current mouse position relative to the top left corner of our render.
        Public Function GetMousePosition() As HellionEngine.Support.Vector
            REM Grabs the cursor's absolute position, it's in relation to the left corner of the entire screen.
            Dim CursorAbsolutePosition As System.Drawing.Point = Windows.Forms.Cursor.Position

            Dim WindowDimensions As New System.Drawing.Size(RenderForm.Width, RenderForm.Height)
            Dim RenderAreaDimensions As New System.Drawing.Size(RenderForm.ClientRectangle.Width, RenderForm.ClientRectangle.Height)
            REM The PixelDifference here is used to try and calculate the offset produced by the window borders
            Dim PixelDifference As System.Drawing.Size = WindowDimensions - RenderAreaDimensions

            REM Window borders on the left and right are split evenly in half
            PixelDifference.Width /= 2
            REM Though the tops and bottoms are not quite half, we try to ammend that in the return
            PixelDifference.Height /= 2

            REM Try and calculate the relative position of our cursor from the top left corner of the rendered area
            Dim WindowAbsolutePosition As System.Drawing.Point = RenderForm.Location + PixelDifference
            Dim CursorRelativePosition As System.Drawing.Point = CursorAbsolutePosition - WindowAbsolutePosition

            REM The -9 is because of the tops and bottoms of the border being not quite the same size
            Return New HellionEngine.Support.Vector(CursorRelativePosition.X, CursorRelativePosition.Y - 9)
        End Function

        REM The Dispose subroutine destroys all of the DirectInput references.
        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.KeyboardInput.Dispose()
            Me.MouseInput.Dispose()
            Me.DirectInput.Dispose()

            HellionEngine.Logging.Write("Input.vb [InstanceClass+Dispose]: Sucessfully disposed.")
        End Sub
    End Class
End Module

Imports System.Windows.Forms
Imports SlimDX
Imports SlimDX.Direct3D9
Imports SlimDX.Windows

Public Module EngineInstance
    Private SingletonInstance As InstanceClass

    Public ReadOnly Property Singleton() As InstanceClass
        Get
            Return SingletonInstance
        End Get
    End Property

    Public Function Instantiate()
        If (SingletonInstance Is Nothing) Then
            SingletonInstance = New InstanceClass
        End If

        Return SingletonInstance
    End Function

    Public Class InstanceClass
        Inherits HellionEngine.Core.EngineObject

        Dim RenderForm As SlimDX.Windows.RenderForm
        Dim Direct3D9 As SlimDX.Direct3D9.Direct3D
        Dim Device As SlimDX.Direct3D9.Device
        Private IsRunning As Boolean = True
        Private IsFocused As Boolean = True
        Private CurrentFrameCount As Integer = 0
        Private InternalFPS As Integer = 0
        Private FPSTimer As HellionEngine.Support.Timing.TimedEvent
        Private LostSimTime As Single

        Private IsShuttingDown As Boolean
        Private MinimumFPS As Integer
        Private MaximumFPS As Integer

        Public Title As String
        Public Width As Integer
        Public Height As Integer
        Public MaximumTimeStep As Single
        Public FPSInTitle As Boolean
        Public MaximumRenderLayers As Integer

        Public AsyncResourceLoadFrameDelay As Integer
        Private AsyncResourceDelayCurrent As Integer
        Private AsyncTextureList As HellionEngine.Support.DynamicArray

        Public Sub New()
            MyBase.New()

            Me.Title = "HellionEngine Project"
            Me.Width = 640
            Me.Height = 480
            Me.MaximumTimeStep = 1
            Me.FPSInTitle = False
            Me.MinimumFPS = 9999
            Me.MaximumFPS = 0
            Me.LostSimTime = 0
            Me.MaximumRenderLayers = 10
            Me.AsyncResourceLoadFrameDelay = 15

            Me.IsShuttingDown = False
        End Sub

        Public ReadOnly Property FPS() As Integer
            Get
                Return Me.InternalFPS
            End Get
        End Property

        Public Sub Start()
            HellionEngine.Logging.Write("HellionEngine")
            HellionEngine.Logging.Write("Copyright (c) Robert MacGregor 2014")

            REM TODO: Load From Config File?
            Me.RenderForm = New SlimDX.Windows.RenderForm(Me.Title)
            Me.RenderForm.DesktopLocation = New System.Drawing.Point(0, 0)
            Me.RenderForm.Width = Me.Width
            Me.RenderForm.Height = Me.Height
            Me.RenderForm.MinimumSize = New System.Drawing.Size(Me.Width, Me.Height)
            Me.RenderForm.MaximumSize = Me.RenderForm.MinimumSize
            Me.RenderForm.MaximizeBox = False

            REM Configure the Presentation Parameters
            Dim PresentParameters As New SlimDX.Direct3D9.PresentParameters()
            PresentParameters.BackBufferWidth = Me.RenderForm.ClientSize.Width
            PresentParameters.BackBufferHeight = Me.RenderForm.ClientSize.Height

            Try
                Me.Direct3D9 = New SlimDX.Direct3D9.Direct3D()
                Me.Device = New SlimDX.Direct3D9.Device(Me.Direct3D9, IntPtr.Zero, SlimDX.Direct3D9.DeviceType.Hardware, Me.RenderForm.Handle, SlimDX.Direct3D9.CreateFlags.HardwareVertexProcessing, PresentParameters)
                ' BattleZone.Game.Globals.Core.DirectXDevice = Me.Device

                Me.Device.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Identity)
                REM Configure the Orthographic Matrix to have origin at the top left corner of the render
                Me.Device.SetTransform(SlimDX.Direct3D9.TransformState.Projection, SlimDX.Matrix.OrthoOffCenterLH(0, Me.RenderForm.Width, Me.RenderForm.Height, 0, 0, 1))

                REM Configure the Render States
                Me.Device.SetRenderState(SlimDX.Direct3D9.RenderState.Lighting, False)
                Me.Device.SetRenderState(SlimDX.Direct3D9.RenderState.AlphaTestEnable, True)
            Catch ex As SlimDX.Direct3D9.Direct3D9Exception
                HellionEngine.Logging.Write("EngineInstance.vb [EngineInstance+Start]: Failed Startup: " + ex.Message)
                System.Windows.Forms.MessageBox.Show(ex.Message, "Failed Startup", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Me.RenderForm.Dispose()
                Return
            End Try

            Me.RenderForm.Show()

            REM Initialize Helpers
            HellionEngine.Input.Instantiate(Me.RenderForm)
            Me.FPSTimer = New HellionEngine.Support.Timing.TimedEvent(1000, True)

            HellionEngine.Rendering.Instantiate(Me.MaximumRenderLayers)
            HellionEngine.Rendering.DeviceInstance = Me.Device

            HellionEngine.Sound.Instantiate(Me.RenderForm)

            HellionEngine.Interaction.Instantiate()

            REM Show the Window and Configure Events
            AddHandler Me.RenderForm.FormClosed, AddressOf Me.ExitHandle
            AddHandler Me.RenderForm.LostFocus, AddressOf Me.LostFocusHandle
            AddHandler Me.RenderForm.GotFocus, AddressOf Me.GotFocusHandle
            AddHandler Me.FPSTimer.OnFire, AddressOf Me.UpdateFPSHandle

            Me.AsyncTextureList = New HellionEngine.Support.DynamicArray(0)
            RaiseEvent OnReady()

            Me.IsRunning = True
            Me.AsyncTextureList.IterationBegin(False)
            While (Me.IsRunning)
                Application.DoEvents()

                If (Me.FPSInTitle) Then
                    Me.RenderForm.Text = Me.Title + " [FPS]: " + Me.FPS.ToString()
                End If

                REM Async Load Resources
                If (Me.AsyncResourceDelayCurrent >= Me.AsyncResourceLoadFrameDelay And Not Me.AsyncTextureList.IterationEnd()) Then
                    Me.AsyncResourceDelayCurrent = 0
                    Dim LoadedTexture As HellionEngine.Rendering.Texture = HellionEngine.Rendering.Singleton.GetTexture(Me.AsyncTextureList.IterationNext())
                End If

                Dim DeltaTimeSeconds As Single = HellionEngine.Support.Timing.DeltaTime()

                If (DeltaTimeSeconds > Me.MaximumTimeStep) Then
                    HellionEngine.Logging.Write("EngineInstance.vb [EngineInstance+Start]: Simulation timestep (" + DeltaTimeSeconds.ToString() + " seconds) exceeded the maximum timestep of (" + Me.MaximumTimeStep + " seconds), using maximum value.")
                    Me.LostSimTime = DeltaTimeSeconds - Me.MaximumTimeStep

                    DeltaTimeSeconds = Me.MaximumTimeStep
                End If

                If (Me.IsRunning) Then
                    If (Me.IsShuttingDown) Then
                        Me.Dispose()
                        Exit While
                    End If

                    REM Update Helpers
                    HellionEngine.Support.Timing.Update(DeltaTimeSeconds)
                    HellionEngine.Interaction.Singleton.OnUpdate(DeltaTimeSeconds)

                    REM Sound and Input is updated independently
                    HellionEngine.Input.Singleton.OnUpdate(DeltaTimeSeconds)
                    HellionEngine.Input.Singleton.OnUpdate(DeltaTimeSeconds)

                    Me.Device.Clear(SlimDX.Direct3D9.ClearFlags.Target, System.Drawing.Color.Black, 0.0, 0)

                    Me.Device.BeginScene()
                    HellionEngine.Rendering.Singleton.OnDraw()
                    Me.Device.EndScene()

                    Me.Device.Present()
                End If

                RaiseEvent OnUpdated(DeltaTimeSeconds)

                REM Prevents the app from eating up all your CPU cycles trying to run the game. Also seems to smooth it over a bit.
                System.Threading.Thread.Sleep(32)

                Me.CurrentFrameCount += 1
                Me.AsyncResourceDelayCurrent += 1
            End While

            REM Do Total EngineObject Check
            HellionEngine.Core.EngineObjectInternal.Cleanup()

            HellionEngine.Logging.Write("EngineInstance.vb [EngineInstance+Start]: Engine shutdown appears successful!")
            HellionEngine.Logging.Write("EngineInstance.vb [EngineInstance+Start]: Max FPS: " + Me.MaximumFPS.ToString + " Min FPS: " + Me.MinimumFPS.ToString() + " Total Lost Sim Time: " + Me.LostSimTime.ToString() + " seconds")
        End Sub

        Public Sub AddAsyncTexture(Filename As String)
            Me.AsyncTextureList.Append(Filename)
        End Sub

        Public Sub AddAsyncAnimation(AnimationName As String)
            Dim directoryPath As String = "Animations/" + AnimationName + "/"
            Dim directory As New System.IO.DirectoryInfo(directoryPath)
            Dim files As System.IO.FileInfo() = directory.GetFiles()

            Dim CurrentFileIndex As Integer = 0
            Dim FileType As String = "jpg"
            For Each File In files
                Me.AddAsyncTexture(directoryPath + File.Name)
            Next File
        End Sub

        Public Sub Shutdown()
            Me.IsShuttingDown = True
        End Sub

        REM Called when the window loses its focus.
        Private Sub LostFocusHandle(ByVal sender As Object, ByVal e As EventArgs)
            Me.IsFocused = False

            RaiseEvent OnLostFocus()
        End Sub

        REM Called when the window gains focus.
        Private Sub GotFocusHandle(ByVal sender As Object, ByVal e As EventArgs)
            Me.IsFocused = True

            RaiseEvent OnGainedFocus()
        End Sub

        REM The Application_Exit Subroutine is only called when the application is terminated for any reason.
        Private Sub ExitHandle(ByVal sender As Object, ByVal e As EventArgs)
            If (Me.IsRunning) Then
                Me.Dispose()
            End If
        End Sub

        Private Sub UpdateFPSHandle()
            Me.InternalFPS = Me.CurrentFrameCount
            Me.CurrentFrameCount = 0

            If (Me.InternalFPS < Me.MinimumFPS) Then
                Me.MinimumFPS = Me.InternalFPS
            End If

            If (Me.InternalFPS > Me.MaximumFPS) Then
                Me.MaximumFPS = Me.InternalFPS
            End If
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            HellionEngine.Logging.Write("EngineInstance.vb [EngineInstance+Dispose]: Attempting Clean Exit ...")
            RaiseEvent OnDispose()

            REM Wind Down...
            Me.RenderForm.Hide()

            HellionEngine.Sound.Singleton.Dispose()

            HellionEngine.Input.Singleton.Dispose()
            HellionEngine.Rendering.Singleton.Dispose()
            HellionEngine.Interaction.Singleton.Dispose()

            Me.Device.Dispose()
            Me.Direct3D9.Dispose()
            Me.RenderForm.Dispose()

            Me.IsRunning = False
            SingletonInstance = Nothing
        End Sub

        Public Event OnUpdated(DeltaTimeSeconds As Single)
        Public Event OnLostFocus()
        Public Event OnGainedFocus()
        Public Event OnReady()
        Public Event OnDispose()
    End Class
End Module
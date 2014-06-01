REM *********************************************************************
REM SoundSystem.vb
REM Module code that is utilized to load and play back sounds.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Public Module Sound
    Private SingletonInstance As ClassInstance

    Public Function Instantiate(RenderForm As SlimDX.Windows.RenderForm)
        If (SingletonInstance Is Nothing) Then
            SingletonInstance = New ClassInstance(RenderForm)
        End If

        Return SingletonInstance
    End Function

    Public ReadOnly Property Singleton() As ClassInstance
        Get
            Return SingletonInstance
        End Get
    End Property


    Public Class ClassInstance
        Inherits HellionEngine.Core.EngineObject

        REM Reference to the DirectSound handle.
        Private DirectSound As SlimDX.DirectSound.DirectSound = Nothing
        REM Array storing all sound buffers.
        Private SoundBuffers As New HellionEngine.Support.DynamicArray(0)
        REM Array storing all known sound sources.
        Private SoundSources As New HellionEngine.Support.DynamicArray(0)
        REM Reference to the sound source playing music.
        Private CurrentMusicSource As SoundSource2D = Nothing

        Private CurrentMusicBuffer As SoundBuffer = Nothing

        Public Sub New(RenderForm As SlimDX.Windows.RenderForm)
            MyBase.New()

            Try
                Me.DirectSound = New SlimDX.DirectSound.DirectSound()
                Me.DirectSound.SetCooperativeLevel(RenderForm.Handle, SlimDX.DirectSound.CooperativeLevel.Priority)
                Me.DirectSound.IsDefaultPool = False
            Catch ex As SlimDX.DirectSound.DirectSoundException
                HellionEngine.Logging.Write("Sound.vb [ClassInstance+New]: Failed to initialize DirectSound! Reason: " + ex.Message)
                Me.DirectSound.Dispose()
                Me.DirectSound = Nothing
                Return
            End Try
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            If (Not (Me.DirectSound Is Nothing)) Then
                For CurrentIndex As Integer = 0 To Me.SoundBuffers.Length - 1
                    Me.SoundBuffers.GetIndex(CurrentIndex).Dispose()
                Next

                For CurrentIndex As Integer = 0 To Me.SoundSources.Length - 1
                    Me.SoundSources.GetIndex(CurrentIndex).Dispose()
                Next

                Me.DirectSound.Dispose()
            End If

            SingletonInstance = Nothing
            HellionEngine.Logging.Write("Sound.vb [ClassInstance+Dispose]: Successfully deinitialized.")
        End Sub

        Public Function GetSound(Filepath As String) As SoundBuffer
            REM Try to find a file associated with NewSoundName first
            SoundBuffers.IterationBegin(False)
            While (Not SoundBuffers.IterationEnd())
                Dim CurrentSoundBuffer As SoundBuffer = SoundBuffers.IterationNext()

                If (CurrentSoundBuffer.Filepath = Filepath) Then
                    Return CurrentSoundBuffer
                End If
            End While

            Dim WavFile As New SlimDX.Multimedia.WaveStream(Filepath)
            Dim ResultBuffer As New SoundBuffer(Filepath, WavFile)

            SoundBuffers.Append(ResultBuffer)
            HellionEngine.Logging.Write("Sound.vb [ClassInstance+LoadSound]: Loaded " + Filepath + ".")
            Return ResultBuffer
        End Function

        Public ReadOnly Property CurrentMusicFile() As String
            Get
                Return Me.CurrentMusicBuffer.Filepath
            End Get
        End Property

        Public Sub PlayMusic(Filename As String, Looping As Boolean)
            If (Not (CurrentMusicSource Is Nothing)) Then
                Me.CurrentMusicSource.Pause()
                Me.CurrentMusicSource.Dispose()
            End If

            Me.CurrentMusicBuffer = Me.GetSound(Filename)
            Me.CurrentMusicSource = PlaySound(CurrentMusicBuffer)
            Me.CurrentMusicSource.Volume = -800
            Me.CurrentMusicSource.Play(Looping)
        End Sub

        Public Function PlaySound(IncomingBuffer As SoundBuffer) As SoundSource2D
            Dim Description As New SlimDX.DirectSound.SoundBufferDescription
            Description.Format = IncomingBuffer.Stream.Format
            Description.AlgorithmFor3D = Guid.Empty
            Description.Flags = SlimDX.DirectSound.BufferFlags.ControlVolume Or SlimDX.DirectSound.BufferFlags.ControlPan
            Description.SizeInBytes = IncomingBuffer.Stream.Length

            REM Dim Buffer As New SlimDX.DirectSound.SecondarySoundBuffer(DirectSound, Description)
            Dim Buffer As New SlimDX.DirectSound.PrimarySoundBuffer(DirectSound, Description)
            Buffer.Write(IncomingBuffer.RawBuffer, 0, SlimDX.DirectSound.LockFlags.EntireBuffer)

            Dim NewSource As New SoundSource2D(Buffer)
            SoundSources.Append(NewSource)

            Return NewSource
        End Function

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            Me.SoundSources.IterationBegin(False)
            While (Not Me.SoundSources.IterationEnd())
                Me.SoundSources.IterationNext().OnUpdate(DeltaTimeSeconds)
            End While
        End Sub

        Public Event OnMusicEnd(RepeatCount As Integer)
    End Class

    REM Helper class that represents a two dimensional sound source.
    Public Class SoundSource2D
        Inherits HellionEngine.Core.EngineObject

        Private InternalLooping As Boolean
        Private InternalLoopCount As Integer
        Private InternalSoundSource As SlimDX.DirectSound.PrimarySoundBuffer

        Public Sub New(NewSoundSource As SlimDX.DirectSound.PrimarySoundBuffer)
            MyBase.New()

            Me.InternalSoundSource = NewSoundSource

            Me.InternalLoopCount = 0
            Me.InternalLooping = False
        End Sub

        Public Property Pan() As Integer
            Get
                Return Me.InternalSoundSource.Pan
            End Get
            Set(value As Integer)
                Me.InternalSoundSource.Pan = value
            End Set
        End Property

        Public Property Looping() As Boolean
            Get
                Return Me.InternalLooping
            End Get
            Set(value As Boolean)
                Me.InternalLooping = True

                If (Not value) Then
                    Me.InternalLoopCount = 0
                    REM Play sound if it's ended and our value is true
                ElseIf (Me.InternalSoundSource.Status = 0) Then
                    Me.Play(True)
                End If
            End Set
        End Property

        Public Sub Play(Optional IsLooping As Boolean = False)
            Me.InternalLooping = IsLooping

            Me.InternalSoundSource.CurrentPlayPosition = 0
            Me.InternalSoundSource.Play(0, SlimDX.DirectSound.PlayFlags.None)
        End Sub

        Public Sub Pause()
            Me.InternalSoundSource.Stop()
        End Sub

        Public Sub Reset()
            Me.InternalSoundSource.CurrentPlayPosition = 0
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            If (Not (Me.InternalSoundSource.Disposed)) Then
                Me.InternalSoundSource.Dispose()
            End If
        End Sub

        Public Property Decibels() As Integer
            Get
                Return Me.InternalSoundSource.Volume
            End Get
            Set(value As Integer)
                Try
                    Me.InternalSoundSource.Volume = value
                Catch ex As SlimDX.DirectSound.DirectSoundException
                    HellionEngine.Logging.Write("Sound.vb [Decibels]: Failed to set sound source volume.")
                End Try
            End Set
        End Property

        Public Property Volume() As Integer
            Get

            End Get
            Set(value As Integer)
                ' Dim NewDecibels As Integer = Math.Floor(10000
            End Set
        End Property

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            REM Check if the sound is not playing
            ' If (Not (Me.InternalSoundSource Is Nothing) And Not Me.InternalSoundSource.Disposed And Me.InternalSoundSource.Status = 0) Then
            'Me.Play()

            ' RaiseEvent OnPlayBackEnd(Me.InternalLoopCount)
            ' Me.InternalLoopCount += 1
            ' End If
        End Sub

        REM 0 = 100%
        REM -300 -> -3 dB -> 50%
        REM 600 -> -6 dB -> 25%
        REM -10000 = 0%

        Public Event OnPlayBackEnd(LoopCount As Integer)
    End Class

    Public Class SoundBuffer
        Inherits HellionEngine.Core.EngineObject

        Public Filepath As String
        Public Stream As SlimDX.Multimedia.WaveStream

        Public RawBuffer() As Byte

        Public Sub New(NewFilepath As String, NewStream As SlimDX.Multimedia.WaveStream)
            MyBase.New()

            Me.Filepath = NewFilepath
            Me.Stream = NewStream

            ReDim Me.RawBuffer(NewStream.Length)
            NewStream.Read(Me.RawBuffer, 0, NewStream.Length)
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.Stream.Dispose()
        End Sub
    End Class
End Module
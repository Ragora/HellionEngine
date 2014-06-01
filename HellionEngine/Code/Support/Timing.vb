REM *********************************************************************
REM Timing.vb
REM Singleton code that is utilized to time specific events.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
    Public Module Timing
        Private InternalSimTimeSeconds As Single
        Private TimedEvents As New HellionEngine.Support.DynamicArray(0)

        Private LastTimeMS As Integer = DateAndTime.Now.Millisecond

        Public Class TimedEvent
            Inherits HellionEngine.Core.EngineObject

            Public Event OnFire()

            Private InternalDelaySeconds As Single
            Private InternalRecurring As Boolean
            Private InternalNextFireTimeSeconds As Single
            Private InternalID As Integer
            Private InternalDisposed As Boolean

            Public Sub New(NewDelayMilliseconds As Single, Recurring As Boolean)
                Me.InternalDelaySeconds = NewDelayMilliseconds / 1000
                Me.InternalRecurring = Recurring

                Me.InternalNextFireTimeSeconds = InternalSimTimeSeconds + Me.InternalDelaySeconds

                Me.InternalID = TimedEvents.Append(Me)
                Me.InternalDisposed = False
            End Sub

            Public ReadOnly Property NextFireTimeSeconds() As Single
                Get
                    Return Me.InternalNextFireTimeSeconds
                End Get
            End Property

            Public Sub Fire()
                If (Not Me.InternalRecurring) Then
                    TimedEvents.RemoveObject(Me.InternalID)
                End If

                Me.InternalNextFireTimeSeconds = InternalSimTimeSeconds + Me.InternalDelaySeconds
                RaiseEvent OnFire()
            End Sub

            Public Overrides Sub Dispose()
                MyBase.Dispose()

                REM Try and prevent an accidental firing
                Me.InternalNextFireTimeSeconds = InternalSimTimeSeconds + 9000
                TimedEvents.RemoveObject(Me.InternalID)
            End Sub
        End Class

        REM Description: Updates the timing manager
        Public Sub Update(DeltaTimeSeconds As Single)
            InternalSimTimeSeconds += DeltaTimeSeconds

            TimedEvents.IterationBegin(False)
            While (Not TimedEvents.IterationEnd())
                Dim CurrentEvent As TimedEvent = TimedEvents.IterationNext()

                If (InternalSimTimeSeconds >= CurrentEvent.NextFireTimeSeconds) Then
                    CurrentEvent.Fire()
                End If
            End While
        End Sub

        REM Description: Returns the delta time in seconds since this function was last called.
        Public ReadOnly Property DeltaTime() As Single
            Get
                Dim DeltaTimeMS As Integer = 0
                Dim CurrentTimeMS As Integer = DateAndTime.Now.Millisecond
                If (CurrentTimeMS < LastTimeMS) Then
                    DeltaTimeMS = CurrentTimeMS - LastTimeMS + 1000
                Else
                    DeltaTimeMS = CurrentTimeMS - LastTimeMS
                End If

                REM Dim DeltaTimeMS As Integer = CurrentTimeMS - LastTimeMS
                LastTimeMS = CurrentTimeMS

                Return DeltaTimeMS / 1000
            End Get
        End Property

        REM Read the current sim time in seconds
        Public ReadOnly Property SimTime() As Single
            Get
                Return InternalSimTimeSeconds
            End Get
        End Property
    End Module
End Namespace
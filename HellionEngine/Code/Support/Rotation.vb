REM *********************************************************************
REM Rotation.vb
REM Rotation helper class that allows one to operate in either degrees
REM or radians without breaking code compatability.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
    Public Class Rotation
        REM Internal Value, stored in Radians
        Private InternalValue As Single = 0.0

        Public Property Radians() As Single
            Get
                Return Me.InternalValue
            End Get
            Set(value As Single)
                REM NOTE: Utilizes recursive calls to fully work out large rotation changes to proper radians (possibly can
                REM modulus but does that work properly on singles?
                If (value < 0) Then
                    Me.Radians = (Math.PI * 2) - Math.Abs(value)
                    Return
                ElseIf (value > (Math.PI * 2)) Then
                    Me.Radians = value - (Math.PI * 2)
                    Return
                End If

                Me.InternalValue = value
            End Set
        End Property

        Public Property Degrees() As Single
            Get
                Return Me.InternalValue * (180 / Math.PI)
            End Get
            Set(value As Single)
                If (value < 0) Then
                    value = (360 - Math.Abs(value))
                ElseIf (value > 360) Then
                    value -= 360
                End If

                Me.Radians = (Math.PI / 180) * value
            End Set
        End Property

        Public Shared Operator +(First As Rotation, Second As Rotation)
            Dim Result As New Rotation()
            Result.Radians = First.Radians + Second.Radians
            Return Result
        End Operator

        Public Shared Operator *(First As Rotation, Second As Rotation)
            Dim Result As New Rotation()
            Result.Radians = First.Radians * Second.Radians
            Return Result
        End Operator

        Public Shared Operator -(First As Rotation, Second As Rotation)
            Dim Result As New Rotation()
            Result.Radians = First.Radians - Second.Radians
            Return Result
        End Operator

        Public Shared Operator /(First As Rotation, Second As Rotation)
            Dim Result As New Rotation()
            Result.Radians = First.Radians / Second.Radians
            Return Result
        End Operator
    End Class
End Namespace
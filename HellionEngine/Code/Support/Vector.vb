REM *********************************************************************
REM Vector.vb
REM Helper class that is utilized to get rid of various direct SlimDX 
REM references in the game and implements various operations and
REM functionality that is not supported by SlimDX's Vector2 class
REM without hacking it.
REM 
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
    Public Class Vector
        Private InternalVector As SlimDX.Vector2

        Public Sub New(X As Single, Y As Single)
            Me.InternalVector = New SlimDX.Vector2(X, Y)
        End Sub

        Public Property X() As Single
            Get
                Return Me.InternalVector.X
            End Get
            Set(value As Single)
                Me.InternalVector = New SlimDX.Vector2(value, Me.InternalVector.Y)
            End Set
        End Property

        Public Property Y() As Single
            Get
                Return Me.InternalVector.Y
            End Get
            Set(value As Single)
                Me.InternalVector = New SlimDX.Vector2(Me.InternalVector.X, value)
            End Set
        End Property

        Public Sub CopyFrom(Other As Vector)
            Me.X = Other.X
            Me.Y = Other.Y
        End Sub

        Public Function Copy() As Vector
            Return New Vector(Me.X, Me.Y)
        End Function

        Public Shared Operator +(First As Vector, Second As Vector) As Vector
            Return New Vector(First.X + Second.X, First.Y + Second.Y)
        End Operator

        Public Shared Operator -(First As Vector) As Vector
            Return New Vector(-First.X, -First.Y)
        End Operator

        Public Shared Operator -(First As Vector, Second As Vector) As Vector
            Return New Vector(First.X - Second.X, First.Y - Second.Y)
        End Operator

        Public Shared Operator /(First As Vector, Second As Vector) As Vector
            Return New Vector(First.X / Second.X, First.Y / Second.Y)
        End Operator

        Public Shared Operator /(First As Vector, Second As Integer) As Vector
            Return New Vector(First.X / Second, First.Y / Second)
        End Operator

        Public Shared Operator *(First As Vector, Second As Vector) As Vector
            Return New Vector(First.X * Second.X, First.Y * Second.Y)
        End Operator

        Public Shared Operator *(First As Vector, Second As Single) As Vector
            Return New Vector(First.X * Second, First.Y * Second)
        End Operator

        Public Function Distance(From As Vector) As Single
            Return Me.Distance(From.X, From.Y)
        End Function

        Public Function Distance(FromX As Single, FromY As Single) As Single
            Return Math.Sqrt(Math.Pow(Me.InternalVector.X - FromX, 2) + Math.Pow(Me.InternalVector.Y - FromY, 2))
        End Function

        Public Function Normalize() As Vector
            Dim Mangnitude As Single = Me.Length()
            Return New Vector(Me.X / Mangnitude, Me.Y / Mangnitude)
        End Function

        Public Function Length() As Single
            Return Math.Sqrt(Math.Pow(Me.X, 2) + Math.Pow(Me.Y, 2))
        End Function

        Public Function SlimDX() As SlimDX.Vector2
            Return New SlimDX.Vector2(Me.X, Me.Y)
        End Function
    End Class
End Namespace
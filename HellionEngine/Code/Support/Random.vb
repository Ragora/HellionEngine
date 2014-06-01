REM *********************************************************************
REM Random.vb
REM Helper module that defines some useful random number generation
REM functions.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
	Public Module Random
    	Public Function RandomInteger(Minimum As Integer, Maximum As Integer) As Integer
        	Return Math.Floor(Rnd() * Maximum + Minimum)
        End Function

		Public Function RandomSingle(Minimum As Single, Maximum As Single) As Single
			Return Rnd() * Maximum + Minimum
		End Function

        Public Function RandomVectorInRadius(Position As HellionEngine.Support.Vector, MinimumRadius As Single, MaximumRadius As Single) As HellionEngine.Support.Vector
            Dim Angle As Single = RandomSingle(0, (2 * Math.PI))
            Return New HellionEngine.Support.Vector(Position.X + (Math.Cos(Angle) * MaximumRadius) + MinimumRadius, Position.Y + (Math.Sin(Angle) * MaximumRadius) + MinimumRadius)
        End Function

        Public Function RandomVectorInRectangle(Rectangle As System.Drawing.Rectangle) As HellionEngine.Support.Vector
            Return New HellionEngine.Support.Vector(RandomSingle(Rectangle.Left, Rectangle.Right), RandomSingle(Rectangle.Top, Rectangle.Bottom))
        End Function
   End Module
End Namespace

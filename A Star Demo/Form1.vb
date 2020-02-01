Public Class frmMain

    'Mode number representations:
    '1: Placing walls
    '2: Removing walls
    '3: Moving the start
    '4: Moving the goal

    Public start As Point = New Point(10, 13)
    Public goal As Point = New Point(16, 13)
    Public walls() As Point
    Public mode As Integer = 0
    Public mouseIsDown As Boolean = False
    Public solving As Boolean = False
    Private demo As map

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Space Or e.KeyCode = Keys.Enter Then
            solving = True
            demo = New map(25)
            For Each i As Point In walls
                demo.setImpassable(i.X, i.Y)
            Next
            demo.setGoal(goal.X, goal.Y)
            demo.setStart(start.X, start.Y)
            picMain.Refresh()
        End If
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReDim walls(0)
        walls(0) = New Point(0, 0)
        picMain.Refresh()
    End Sub

    Private Sub picMain_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseDown
        mouseIsDown = True
        Dim simple As Point = New Point(Math.Floor(e.X / 25) + 1, Math.Floor(e.Y / 25) + 1)
        If walls.Contains(simple) Then
            mode = 1
        ElseIf simple = start Then
            mode = 2
        ElseIf simple = goal Then
            mode = 3
        Else
            mode = 0
        End If
    End Sub

    Private Sub picMain_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseMove
        If mouseIsDown = True Then
            Dim simple As Point = New Point(Math.Floor(e.X / 25) + 1, Math.Floor(e.Y / 25) + 1)
            If mode = 0 Then
                If simple <> start And simple <> goal Then
                    ReDim Preserve walls(walls.Length)
                    walls(walls.Length - 1) = simple
                End If
            ElseIf mode = 1 Then
                If walls.Length >= 1 And Array.IndexOf(walls, simple) >= 0 And Array.IndexOf(walls, simple) < walls.Length Then
                    For i As Integer = 0 To walls.Length - 1
                        If i > Array.IndexOf(walls, simple) Then
                            walls(i - 1) = walls(i)
                        End If
                    Next
                    ReDim Preserve walls(walls.Length - 2)
                End If
            ElseIf mode = 2 Then
                start = simple
            ElseIf mode = 3 Then
                goal = simple
            End If
            picMain.Refresh()
        End If
    End Sub

    Private Sub picMain_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseUp
        mouseIsDown = False
        Dim simple As Point = New Point(Math.Floor(e.X / 25) + 1, Math.Floor(e.Y / 25) + 1)
        If mode = 0 Then
            If simple <> start And simple <> goal Then
                ReDim Preserve walls(walls.Length)
                walls(walls.Length - 1) = simple
            End If
        ElseIf mode = 1 Then
            If walls.Length >= 1 And Array.IndexOf(walls, simple) >= 0 And Array.IndexOf(walls, simple) < walls.Length Then
                For i As Integer = 0 To walls.Length - 1
                    If i > Array.IndexOf(walls, simple) Then
                        walls(i - 1) = walls(i)
                    End If
                Next
                ReDim Preserve walls(walls.Length - 2)
            End If
        ElseIf mode = 2 Then
            start = simple
        ElseIf mode = 3 Then
            goal = simple
        End If
        picMain.Refresh()
    End Sub

    Private Sub picMain_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles picMain.Paint

        If solving = True Then
            demo.getPath()
            For Each i As Point In demo.getOpenList
                e.Graphics.FillRectangle(Brushes.LightGreen, (i.X - 1) * 25, (i.Y - 1) * 25, 25, 25)
            Next
            For Each i As Point In demo.getClosedList
                e.Graphics.FillRectangle(Brushes.SkyBlue, (i.X - 1) * 25, (i.Y - 1) * 25, 25, 25)
            Next
            If demo.isSolvable = True Then
                Dim linePen As Pen = New Pen(Color.Yellow, 3)
                solving = False
                Dim current As Point = New Point(demo.getStartTile().coordinate.X, demo.getStartTile().coordinate.Y)
                For Each direction As ArrowDirection In demo.getDirections()
                    Select Case direction
                        Case ArrowDirection.Left
                            e.Graphics.DrawLine(linePen, (current.X - 1) * 25 + 13, (current.Y - 1) * 25 + 13, (current.X - 1) * 25 - 25 + 13, (current.Y - 1) * 25 + 13)
                            current = New Point(current.X - 1, current.Y)
                        Case ArrowDirection.Right
                            e.Graphics.DrawLine(linePen, (current.X - 1) * 25 + 13, (current.Y - 1) * 25 + 13, (current.X - 1) * 25 + 25 + 13, (current.Y - 1) * 25 + 13)
                            current = New Point(current.X + 1, current.Y)
                        Case ArrowDirection.Up
                            e.Graphics.DrawLine(linePen, (current.X - 1) * 25 + 13, (current.Y - 1) * 25 + 13, (current.X - 1) * 25 + 13, (current.Y - 1) * 25 - 25 + 13)
                            current = New Point(current.X, current.Y - 1)
                        Case ArrowDirection.Down
                            e.Graphics.DrawLine(linePen, (current.X - 1) * 25 + 13, (current.Y - 1) * 25 + 13, (current.X - 1) * 25 + 13, (current.Y - 1) * 25 + 25 + 13)
                            current = New Point(current.X, current.Y + 1)
                    End Select
                Next
            End If
        End If

        e.Graphics.FillRectangle(Brushes.Green, (start.X - 1) * 25, (start.Y - 1) * 25, 25, 25)
        e.Graphics.FillRectangle(Brushes.Red, (goal.X - 1) * 25, (goal.Y - 1) * 25, 25, 25)
        For Each i As Point In walls
            e.Graphics.FillRectangle(Brushes.Gray, (i.X - 1) * 25, (i.Y - 1) * 25, 25, 25)
        Next

        Dim tempx As Integer = 0
        Dim tempy As Integer = 0
        While tempx <= 625
            tempy = 0
            While tempy <= 625
                e.Graphics.DrawLine(Pens.Gray, 0, tempy, 625, tempy)
                tempy += 25
            End While
            e.Graphics.DrawLine(Pens.Gray, tempx, 0, tempx, 625)
            tempx += 25
        End While
    End Sub
End Class

Class map
    Class tile
        Public Property start As Boolean
            Get
                Return start
            End Get
            Set(ByVal value As Boolean)
                If value = True Then
                    impassable = False
                    goal = False
                End If
                start = value
            End Set
        End Property
        Public Property goal As Boolean
            Get
                Return goal
            End Get
            Set(ByVal value As Boolean)
                If value = True Then
                    impassable = False
                    start = False
                End If
                goal = value
            End Set
        End Property
        Public Property impassable As Boolean
            Get
                Return impassable
            End Get
            Set(ByVal value As Boolean)
                If value = True Then
                    start = False
                    goal = False
                End If
                impassable = value
            End Set
        End Property
        Public Property parent As tile
        Public Property g As Integer
        Public Property h As Integer
        Public Property f As Integer
        Public Property coordinate As System.Drawing.Point
        Public Sub updateGF()
            If Me.start = True Then
                Me.g = 0
                Me.f = Me.h
            ElseIf Me.impassable = False And IsNothing(Me.parent) = False Then
                Me.g = Me.parent.g + 1
                Me.f = Me.g + Me.h
            Else
                Me.g = 0
                Me.f = 0
            End If
        End Sub
    End Class

#Region "Values"
    'Grid tiles sorted as x val, y val
    Protected grid()() As tile
    Protected openList() As tile
    Protected closedList() As tile
    Protected solved As Boolean
#End Region

#Region "Subroutines to update values"
    'Find H value
    Private Sub resetHGF()
        Dim goal As System.Drawing.Point = getGoalTile().coordinate
        For i As Integer = 0 To grid.Length - 1
            For k As Integer = 0 To grid(i).Length - 1
                If grid(i)(k).impassable = False And grid(i)(k).goal = False Then
                    grid(i)(k).h = Math.Abs((i + 1) - goal.X) + Math.Abs((k + 1) - goal.Y)
                Else
                    grid(i)(k).h = 0
                End If
                grid(i)(k).g = 0
                grid(i)(k).f = 0
                grid(i)(k).parent = Nothing
            Next
        Next
    End Sub

    'Find the parent of the specified tile
    Private Sub getParent(ByVal tile As tile)
        If tile.impassable = False Then
            Dim parent As tile
            If IsNothing(tile.parent) = False Then
                parent = tile.parent
            End If
            For Each i As tile In getNearby(tile)
                If closedList.Contains(i) Then
                    If IsNothing(parent) = True Then
                        parent = i
                    ElseIf i.g + 1 > parent.g Then
                        parent = i
                    End If
                End If
            Next
            If IsNothing(parent) = False Then
                tile.parent = parent
            End If
        End If
    End Sub

    'Update the closed list to be only the starting point
    Private Sub resetLists()
        ReDim closedList(0)
        closedList(0) = getStartTile()
        Dim startTile As tile = getStartTile()
        startTile.updateGF()
        ReDim openList(0)
        For Each tile As tile In getNearby(startTile)
            If tile.impassable = False Then
                openList(openList.Length - 1) = tile
                ReDim Preserve openList(openList.Length)
            End If
        Next
        ReDim Preserve openList(openList.Length - 2)
        updateOpenList()
        For i As Integer = 0 To grid.Length - 1
            For k As Integer = 0 To grid(i).Length - 1
                If openList.Contains(grid(i)(k)) Then
                    getParent(grid(i)(k))
                    grid(i)(k).updateGF()
                End If
            Next
        Next
        updateOpenList()
    End Sub

    'Get the next closed list tile
    Private Sub getNextClosed()
        'Find lowest f value in open list
        Dim lowest As tile
        For Each tile As tile In openList
            If IsNothing(lowest) = True Then
                lowest = tile
            ElseIf lowest.f > tile.f Then
                lowest = tile
            End If
        Next

        If IsNothing(lowest) = False Then
            'Put lowest in closed list and remove from open list
            ReDim Preserve closedList(closedList.Length)
            closedList(closedList.Length - 1) = lowest
            Dim index As Integer = Array.IndexOf(openList, lowest)
            For i As Integer = 0 To openList.Length - 1
                If i > index Then
                    openList(i - 1) = openList(i)
                End If
            Next
            ReDim Preserve openList(openList.Length - 2)

            'Add nearby tiles to lowest to the open list (If not already in open list)
            Dim nearby() As tile = getNearby(lowest)
            For Each tile As tile In nearby
                If openList.Contains(tile) = False And closedList.Contains(tile) = False And tile.impassable = False Then
                    ReDim Preserve openList(openList.Length)
                    openList(openList.Length - 1) = tile
                End If
            Next
            updateOpenList()
        End If
    End Sub

    'Update the open list tiles
    Private Sub updateOpenList()
        For i As Integer = 0 To grid.Length - 1
            For k As Integer = 0 To grid(i).Length - 1
                For e As Integer = 0 To openList.Length - 1
                    If openList(e).coordinate.X = grid(i)(k).coordinate.X And openList(e).coordinate.Y = grid(i)(k).coordinate.Y Then
                        getParent(grid(i)(k))
                        grid(i)(k).updateGF()
                        openList(e) = grid(i)(k)
                    End If
                Next
            Next
        Next
    End Sub
#End Region

#Region "Gets for use outside of class"
    'Get open list as points
    Public Function getOpenList() As Point()
        Dim returnVal(0) As Point
        For Each i As tile In openList
            returnVal(returnVal.Length - 1) = i.coordinate
            ReDim Preserve returnVal(returnVal.Length)
        Next
        ReDim Preserve returnVal(returnVal.Length - 2)
        Return returnVal
    End Function

    'Get closed list as points
    Public Function getClosedList() As Point()
        Dim returnVal(0) As Point
        For Each i As tile In closedList
            returnVal(returnVal.Length - 1) = i.coordinate
            ReDim Preserve returnVal(returnVal.Length)
        Next
        ReDim Preserve returnVal(returnVal.Length - 2)
        Return returnVal
    End Function

    'Get path directions
    Public Function getDirections() As System.Windows.Forms.ArrowDirection()
        If Me.solved = False Then
            getPath()
        End If
        Dim directions(0) As System.Windows.Forms.ArrowDirection
        If IsNothing(getGoalTile().parent) = False Then
            Dim currentTile As tile = getGoalTile()
            While currentTile.Equals(getStartTile()) = False
                If currentTile.coordinate.X > currentTile.parent.coordinate.X Then
                    directions(directions.Length - 1) = Windows.Forms.ArrowDirection.Right
                    ReDim Preserve directions(directions.Length)
                ElseIf currentTile.coordinate.X < currentTile.parent.coordinate.X Then
                    directions(directions.Length - 1) = Windows.Forms.ArrowDirection.Left
                    ReDim Preserve directions(directions.Length)
                ElseIf currentTile.coordinate.Y > currentTile.parent.coordinate.Y Then
                    directions(directions.Length - 1) = Windows.Forms.ArrowDirection.Down
                    ReDim Preserve directions(directions.Length)
                ElseIf currentTile.coordinate.Y < currentTile.parent.coordinate.Y Then
                    directions(directions.Length - 1) = Windows.Forms.ArrowDirection.Up
                    ReDim Preserve directions(directions.Length)
                End If
                currentTile = currentTile.parent
            End While
            ReDim Preserve directions(directions.Length - 2)
        Else
            directions(0) = Nothing
        End If
        Array.Reverse(directions)
        Return directions
    End Function

    'Check if the goal can be attained
    Public Function isSolvable()
        If getDirections().Length > 1 Or getNearby(getGoalTile).Contains(getStartTile) Then
            Return True
        Else
            Return False
        End If
    End Function

    'Finds the path
    Public Sub getPath()
        Me.solved = True
        While IsNothing(getGoalTile().parent) = True And openList.Length > 0
            getNextClosed()
        End While
    End Sub
#End Region

#Region "Tile gets"
    Public Function getStartTile() As tile
        For i As Integer = 0 To grid.Length - 1
            For k As Integer = 0 To grid(i).Length - 1
                If grid(i)(k).start = True Then
                    Return grid(i)(k)
                End If
            Next
        Next
        Return grid(0)(0)
    End Function

    Public Function getGoalTile() As tile
        For i As Integer = 0 To grid.Length - 1
            For k As Integer = 0 To grid(i).Length - 1
                If grid(i)(k).goal = True Then
                    Return grid(i)(k)
                End If
            Next
        Next
        Return grid(0)(0)
    End Function

    'Find tiles next to chosen tile
    Private Function getNearby(ByVal tile As tile) As tile()
        Dim nearby(0) As tile
        If tile.coordinate.X > 1 Then
            nearby(nearby.Length - 1) = grid(tile.coordinate.X - 2)(tile.coordinate.Y - 1)
            ReDim Preserve nearby(nearby.Length)
        End If
        If tile.coordinate.Y > 1 Then
            nearby(nearby.Length - 1) = grid(tile.coordinate.X - 1)(tile.coordinate.Y - 2)
            ReDim Preserve nearby(nearby.Length)
        End If
        If tile.coordinate.X < grid.Length Then
            nearby(nearby.Length - 1) = grid(tile.coordinate.X)(tile.coordinate.Y - 1)
            ReDim Preserve nearby(nearby.Length)
        End If
        If tile.coordinate.Y < grid(0).Length Then
            nearby(nearby.Length - 1) = grid(tile.coordinate.X - 1)(tile.coordinate.Y)
            ReDim Preserve nearby(nearby.Length)
        End If
        ReDim Preserve nearby(nearby.Length - 2)
        Return nearby
    End Function
#End Region

#Region "Tile sets"
    'Constructor (New map has specified grid size)
    Public Sub New(ByVal gridLength As Integer, Optional ByVal gridWidth As Integer = -1)
        If gridLength < 0 Then
            gridLength = 1
        End If
        If gridWidth < 0 Then
            gridWidth = gridLength
        End If
        'Initialise tiles
        ReDim grid(gridLength - 1)
        For i As Integer = 0 To gridLength - 1
            ReDim grid(i)(gridWidth - 1)
            For k As Integer = 0 To gridWidth - 1
                grid(i)(k) = New tile
                grid(i)(k).start = False
                grid(i)(k).goal = False
                grid(i)(k).impassable = False
                grid(i)(k).coordinate = New System.Drawing.Point(i + 1, k + 1)
            Next
        Next
        grid(0)(0).start = True
        grid(0)(0).goal = True
        resetHGF()
        resetLists()
        Me.solved = False
    End Sub

    'Make only specified tile start
    Public Sub setStart(ByVal x As Integer, ByVal y As Integer)
        If x <= grid.Length And y <= grid.Length And x > 0 And y > 0 Then
            For i As Integer = 0 To grid.Length - 1
                For k As Integer = 0 To grid(i).Length - 1
                    grid(i)(k).start = False
                Next
            Next
            grid(x - 1)(y - 1).start = True
        End If
        resetHGF()
        resetLists()
        Me.solved = False
    End Sub

    'Make only specified tile goal
    Public Sub setGoal(ByVal x As Integer, ByVal y As Integer)
        If x <= grid.Length And y <= grid.Length And x > 0 And y > 0 Then
            For i As Integer = 0 To grid.Length - 1
                For k As Integer = 0 To grid(i).Length - 1
                    grid(i)(k).goal = False
                Next
            Next
            grid(x - 1)(y - 1).goal = True
        End If
        resetHGF()
        resetLists()
        Me.solved = False
    End Sub

    'Sets a tile to impassable
    Public Sub setImpassable(ByVal x As Integer, ByVal y As Integer)
        If x <= grid.Length And y <= grid.Length And x > 0 And y > 0 Then
            grid(x - 1)(y - 1).impassable = True
        End If
        resetHGF()
        resetLists()
        Me.solved = False
    End Sub

    'Resets all impassable tiles
    Public Sub resetImpassables()
        For i As Integer = 0 To grid.Length - 1
            For k As Integer = 0 To grid(i).Length - 1
                grid(i)(k).impassable = False
            Next
        Next
        resetHGF()
        resetLists()
        Me.solved = False
    End Sub
#End Region

#Region "Debugging"
    '----- NOTE -----
    '
    'All of the following function are printed to the console

    'Produces a visual map of the problem
    'Start: X
    'Goal: *
    'Impassable tile: ~
    'Regular tile: O
    Public Sub visualMap()
        Console.WriteLine("Visual map")
        Console.Write("   ")
        For i As Integer = 1 To grid.Length
            Console.Write(Format(i, "0#") & " ")
        Next
        Console.WriteLine()
        For k As Integer = 0 To Me.grid(0).Length - 1
            Console.Write(Format(k + 1, "0#") & " ")
            For i As Integer = 0 To Me.grid.Length - 1
                If Me.grid(i)(k).start = True Then
                    Console.Write("X")
                ElseIf Me.grid(i)(k).goal = True Then
                    Console.Write("*")
                ElseIf Me.grid(i)(k).impassable = True Then
                    Console.Write("~")
                Else
                    Console.Write("O")
                End If
                Console.Write("  ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

    'Produces a map with each tiles heuristic value
    Public Sub heuristicMap()
        Console.WriteLine("Heuristic mapping")
        For k As Integer = 0 To Me.grid(0).Length - 1
            For i As Integer = 0 To Me.grid.Length - 1
                Console.Write(Format(Me.grid(i)(k).h, "0#"))
                Console.Write(" ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

    'Produces a map with each tile and the list it belongs to
    'Open list: O
    'Closed list: C
    'Not listed: N
    Public Sub listsMap()
        If Me.solved = False Then
            getPath()
        End If
        Console.WriteLine("List mapping")
        For k As Integer = 0 To Me.grid(0).Length - 1
            For i As Integer = 0 To Me.grid.Length - 1
                If Me.openList.Contains(Me.grid(i)(k)) Then
                    Console.Write("O")
                ElseIf Me.closedList.Contains(Me.grid(i)(k)) Then
                    Console.Write("C")
                Else
                    Console.Write("N")
                End If
                Console.Write(" ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

    'Produces the visual map, but adds the directions to get from start to goal
    'Uses cardinal directions (Up: N, Right: E, Down: S, Left: W)
    Public Sub directionMap()
        If Me.solved = False Then
            getPath()
        End If
        Dim temp(Me.grid.Length - 1)() As String
        For i As Integer = 0 To Me.grid.Length - 1
            ReDim temp(i)(Me.grid(0).Length)
            For k As Integer = 0 To Me.grid(0).Length - 1
                If Me.grid(i)(k).impassable = True Then
                    temp(i)(k) = "~"
                ElseIf Me.grid(i)(k).start = True Then
                    temp(i)(k) = "X"
                ElseIf Me.grid(i)(k).goal = True Then
                    temp(i)(k) = "*"
                Else
                    temp(i)(k) = "O"
                End If
            Next
        Next
        Dim currentTile As tile = Me.getGoalTile()
        Me.printDirections()
        Dim directions() As System.Windows.Forms.ArrowDirection = getDirections()
        For Each i As Windows.Forms.ArrowDirection In directions
            Select Case i
                Case Windows.Forms.ArrowDirection.Left
                    temp(currentTile.coordinate.X - 1)(currentTile.coordinate.Y - 1) = "W"
                    currentTile = currentTile.parent
                Case Windows.Forms.ArrowDirection.Right
                    temp(currentTile.coordinate.X - 1)(currentTile.coordinate.Y - 1) = "E"
                    currentTile = currentTile.parent
                Case Windows.Forms.ArrowDirection.Up
                    temp(currentTile.coordinate.X - 1)(currentTile.coordinate.Y - 1) = "N"
                    currentTile = currentTile.parent
                Case Windows.Forms.ArrowDirection.Down
                    temp(currentTile.coordinate.X - 1)(currentTile.coordinate.Y - 1) = "S"
                    currentTile = currentTile.parent
            End Select
        Next
        temp(Me.getGoalTile().coordinate.X - 1)(Me.getGoalTile().coordinate.Y - 1) = "*"
        Console.WriteLine("Direction mapping")
        For k As Integer = 0 To Me.grid(0).Length - 1
            For i As Integer = 0 To Me.grid.Length - 1
                Console.Write(temp(i)(k))
                Console.Write(" ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

    'Prints a list of every tile with a parent, listing its location and its parent's location
    Public Sub printParents()
        If Me.solved = False Then
            getPath()
        End If
        Console.WriteLine("Parents:")
        Dim currentTile As tile = getGoalTile()
        While currentTile.Equals(getStartTile()) = False
            Console.Write(currentTile.coordinate.X.ToString & ", " & currentTile.coordinate.Y.ToString)
            Console.Write(" Parent: ")
            Console.WriteLine(currentTile.parent.coordinate.X.ToString & ", " & currentTile.parent.coordinate.Y.ToString)
            currentTile = currentTile.parent
        End While
    End Sub

    'Prints a list of the directions in order to get from start to goal
    Public Sub printDirections()
        If Me.solved = False Then
            getPath()
        End If
        Console.WriteLine("Directions:")
        For Each direction As ArrowDirection In getDirections()
            Console.WriteLine(direction.ToString)
        Next
        Console.WriteLine()
    End Sub

    'Prints the location of every tile on the open list and closed list
    Public Sub printLists()
        Console.WriteLine("Closed list:")
        For Each i As tile In Me.closedList
            Console.WriteLine(i.coordinate.X & ", " & i.coordinate.Y & " : ")
            If i.start = False Then
                Console.WriteLine("Parent: " & i.parent.coordinate.X.ToString & ", " & i.parent.coordinate.Y.ToString)
            Else
                Console.WriteLine("Is start tile")
            End If
            Console.WriteLine("H: " & i.h.ToString & " G: " & i.g.ToString & " F: " & i.f.ToString)
            Console.WriteLine()
        Next
        Console.WriteLine()
        Console.WriteLine("Open list:")
        For Each i As tile In Me.openList
            Console.WriteLine(i.coordinate.X & ", " & i.coordinate.Y & " : ")
            Console.WriteLine("Parent: " & i.parent.coordinate.X.ToString & ", " & i.parent.coordinate.Y.ToString)
            Console.WriteLine("H: " & i.h.ToString & " G: " & i.g.ToString & " F: " & i.f.ToString)
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub
#End Region
End Class

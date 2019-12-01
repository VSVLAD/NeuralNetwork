Imports NeuralProject

Public Class Form1

    Dim nn As New NeuralNetwork(2, 60, 40, 1)

    Dim userDrawFiltered As New List(Of Point)
    Dim userDraw As New List(Of Point)
    Dim networkDraw As New List(Of Point)

    Dim flagRun As Boolean = False
    Dim modSpeed As Integer = 1

    Dim bmpBuffer As New Bitmap(500, 500)
    Dim g As Graphics

    Dim lr As Double = 0.1
    Dim epoch As Integer
    Dim avgError As Double

    ' Перерисовка графики
    Public Sub RedrawLines(graph As Graphics)
        Dim penUser As New Pen(Brushes.BlueViolet, 4)
        Dim penNetwork As New Pen(Brushes.OrangeRed, 4)

        Dim brushUserTraining As Brush = Brushes.Red

        g.Clear(Color.White)
        g.DrawString($"Эпоха: {epoch}", DefaultFont, Brushes.Black, 20, 20)
        g.DrawString($"Ошибка: {(avgError / networkDraw.Count).ToString("0.################")}", DefaultFont, Brushes.Black, 20, 50)

        For I = 1 To userDraw.Count - 1
            graph.DrawLine(penUser, userDraw(I - 1), userDraw(I))
        Next

        For I = 1 To userDrawFiltered.Count - 1
            graph.FillEllipse(brushUserTraining, userDrawFiltered(I).X - 4, userDrawFiltered(I).Y - 4, 8, 8)
        Next

        For I = 1 To networkDraw.Count - 1
            graph.DrawLine(penNetwork, networkDraw(I - 1), networkDraw(I))
        Next

        g.DrawImage(bmpBuffer, 0, 0)
        pboxDraw.Image = bmpBuffer
    End Sub

    ' Вычисляем что надумала сеть по пользовательским данным
    Public Sub PredictByUserDraw()
        Dim stepIndex = 0
        networkDraw.Clear()

        For Each pnt In userDrawFiltered
            stepIndex += 1

            Dim inputX = NeuralConvert.Scaler(pnt.X, 0, 500, 0, 1)
            Dim inputStep = NeuralConvert.Scaler(stepIndex, 0, 100, 0, 1)

            Dim networkY = NeuralConvert.Scaler(nn.Predict({inputX, inputStep})(0), 0, 1, 0, 500)
            Dim resultPoint As New Point(pnt.X, networkY)

            networkDraw.Add(resultPoint)
        Next
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        nn.Biases(0) = 1

        g = Graphics.FromImage(bmpBuffer)
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
    End Sub

    Private Sub pboxDraw_MouseDown(sender As Object, e As MouseEventArgs) Handles pboxDraw.MouseDown
        If e.Button = MouseButtons.Left Then
            userDraw.Add(e.Location)
            RedrawLines(g)
        End If
    End Sub


    Private Sub pboxDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles pboxDraw.MouseMove
        If e.Button = MouseButtons.Left Then
            userDraw.Add(e.Location)
            RedrawLines(g)
        End If
    End Sub

    Private Sub pboxDraw_Paint(sender As Object, e As PaintEventArgs) Handles pboxDraw.Paint
        'RedrawLines(e.Graphics)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        flagRun = True
        Button2.Enabled = True
        Button3.Enabled = False

        ' Выбираем частично точки
        userDrawFiltered.Clear()

        For I = 0 To userDraw.Count - 1 Step userDraw.Count / 20
            userDrawFiltered.Add(userDraw(I))
        Next

        ' По эпохам
        For epoch = 0 To 5000000
            avgError = 0.0

            ' По выбранным точкам пользователя
            Dim stepIndex As Integer = 0
            For Each pnt In userDrawFiltered
                stepIndex += 1

                Dim scaledStep As Double = NeuralConvert.Scaler(stepIndex, 0, 100, 0, 1)
                Dim scaledX As Double = NeuralConvert.Scaler(pnt.X, 0, 500, 0, 1)
                Dim scaledY As Double = NeuralConvert.Scaler(pnt.Y, 0, 500, 0, 1)

                avgError += nn.Training({scaledX, scaledStep}, {scaledY}, lr)
            Next

            ' Смотрим чему обучилась. Прогнозируем
            If epoch Mod modSpeed = 0 Then

                ' Рисуем
                PredictByUserDraw()
                RedrawLines(g)

                Application.DoEvents()
            End If

            If Not flagRun Then Exit For
        Next

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        flagRun = False

        Button1.Enabled = True
        Button2.Enabled = False
        Button3.Enabled = True
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        Button1.Enabled = True
        Button2.Enabled = False
        Button3.Enabled = False

        userDraw.Clear()
        userDrawFiltered.Clear()
        networkDraw.Clear()

        nn.regenerateWeights()
    End Sub

    Private Sub TrackBar1_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar1.ValueChanged
        Label2.Text = TrackBar1.Value / 100
        lr = TrackBar1.Value / 100
    End Sub

    Private Sub TrackBar2_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar2.ValueChanged
        modSpeed = TrackBar2.Value
    End Sub

    ' Обучение по квадрату
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        flagRun = True
        Button2.Enabled = True
        Button3.Enabled = False

        ' Выбираем точки
        userDrawFiltered.Clear()
        userDrawFiltered.Add(New Point(100, 100))
        userDrawFiltered.Add(New Point(100, 200))
        userDrawFiltered.Add(New Point(100, 300))
        userDrawFiltered.Add(New Point(200, 300))
        userDrawFiltered.Add(New Point(300, 300))
        userDrawFiltered.Add(New Point(300, 200))
        userDrawFiltered.Add(New Point(300, 100))
        userDrawFiltered.Add(New Point(200, 100))
        userDrawFiltered.Add(New Point(100, 100))

        lr = 0.085

        ' По эпохам
        For epoch = 0 To 5000000
            avgError = 0.0

            ' По выбранным точкам пользователя
            Dim stepIndex As Integer = 0
            For Each pnt In userDrawFiltered
                stepIndex += 1

                Dim scaledStep As Double = NeuralConvert.Scaler(stepIndex, 0, 100, 0, 1)
                Dim scaledX As Double = NeuralConvert.Scaler(pnt.X, 0, 500, 0, 1)
                Dim scaledY As Double = NeuralConvert.Scaler(pnt.Y, 0, 500, 0, 1)

                avgError += nn.Training({scaledX, scaledStep}, {scaledY}, lr)
            Next

            ' Смотрим чему обучилась. Прогнозируем
            If epoch Mod modSpeed = 0 Then

                ' Рисуем
                PredictByUserDraw()
                RedrawLines(g)

                Application.DoEvents()
            End If

            If Not flagRun Then Exit For
        Next
    End Sub

    ' Добавить рандомно точку в середину данных
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        userDrawFiltered.Insert(5, New Point(400 + Int(Rnd() * 100), 400 + Int(Rnd() * 100)))
    End Sub

    ' Отключить нейрончик
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        For ind = 0 To nn.Activities(1).Length - 1

            ' Восстановить всё
            For ind2 = 0 To nn.Activities(1).Length - 1
                nn.Activities(1)(ind2) = 1
            Next

            Me.Text = $"Во втором слое отключен нейрон № {ind + 1} из {nn.Activities(1).Length}"
            nn.Activities(1)(ind) = 0

            PredictByUserDraw()
            RedrawLines(g)

            Application.DoEvents()
            Threading.Thread.Sleep(500)
        Next

    End Sub

    ' Отрисовать изученный квадрат
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        PredictByUserDraw()
        RedrawLines(g)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        NeuralState.Save("State.txt", nn)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        nn = NeuralState.Load("State.txt")
    End Sub



End Class

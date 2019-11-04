Imports NeuralProject


Module Program

    Public Sub Main()

        ' 0  0    0
        ' 0  1    1
        ' 1  0    1
        ' 1  1    0

        Dim NN As New Network(2, 4, 1)
        NN.LeaningRate = 0.1

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 100000)
            NN.AverageQuadError = 0.0

            NN.TrainingSet({0, 0}, {0})
            NN.TrainingSet({0, 1}, {1})
            NN.TrainingSet({1, 0}, {1})
            NN.TrainingSet({1, 1}, {0})

            NN.AverageQuadError = NN.AverageQuadError / 4

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, NN.AverageQuadError.ToString("##0.##########"))
        Next

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 0 и 0. Ожидаем 0. Решение {0}", NN.Predict({0, 0})(0))
        Console.WriteLine("Проверка на значениях: 0 и 1. Ожидаем 1. Решение {0}", NN.Predict({0, 1})(0))
        Console.WriteLine("Проверка на значениях: 1 и 0. Ожидаем 1. Решение {0}", NN.Predict({1, 0})(0))
        Console.WriteLine("Проверка на значениях: 1 и 1. Ожидаем 0. Решение {0}", NN.Predict({1, 1})(0))
        Console.ReadLine()
    End Sub

End Module

Option Explicit On
Option Strict On

Namespace NeuralProject.Interfaces

    ''' <summary>Интерфейс для создания, обучения и рассчета нейронной сети</summary>
    Public Interface INetwork

        ''' <summary>Событие используется, когда завершается передача сигнала в следующий слой</summary>
        ''' <param name="LayerIndex">Номер слоя</param>
        Event ForwardComplete(LayerIndex As Integer)

        ''' <summary>Событие используется, когда завершается передача ошибки в предыдущий слой</summary>
        ''' <param name="LayerIndex">Номер слоя</param>
        Event BackwardComplete(LayerIndex As Integer)

        ''' <summary>Метод создаёт базовую структуру из массивов для нейронной сети</summary>
        ''' <param name="NeuronCount">
        ''' Первый элемент - количество нейронов в входном слое
        ''' Последний элемент - количество нейронов в выходном слое
        ''' Остальные элементы - количество слоёв и количество нейронов в них
        ''' </param>
        ''' <returns>Возвращает количество созданных слоёв</returns>
        Function Regenerate(ParamArray NeuronCount() As Integer) As Integer

        ''' <summary>Передаём обучающий сет и тренируем сеть</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <param name="TargetValues">Массив ожидаемых ответов</param>
        ''' <param name="LearningRate">Коэффициент обучения сети</param>
        ''' <returns>Возвращает текущую среднеквадратичную ошибку</returns>
        Function Training(InputValues() As Double, TargetValues() As Double, LearningRate As Double) As Double

        ''' <summary>Передаём исходные данные и рассчитываем результат сети</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <returns>Возвращает массив значений выходных нейронов</returns>
        Function Predict(InputValues() As Double) As Double()

        ''' <summary>
        ''' Массив со списком значений нейронов (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является итоговое значение нейрона
        ''' </summary>
        Property Neurons As Double()()

        ''' <summary>
        ''' Массив со списком весов (y)(m)(n)
        ''' y - номер между текущим слоем и следующим
        ''' m - номер нейрона из текущего слоя
        ''' n - номер нейрона из следующего слоя
        ''' (m)(n) - значением является переданный вес
        ''' </summary>
        Property Weights As Double()()()

        ''' <summary>
        ''' Массив со списком ошибок (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является переданная ошибка
        ''' </summary>
        Property Errors As Double()()

        ''' <summary>
        ''' Массив со списком активационных функций (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является класс реализующий интерфейс активационной функции IFunction
        ''' </summary>
        Property Activators As IFunction()()

        ''' <summary>
        ''' Массив со списком задействованных нейронов (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - 1 - сигнал нейрона будет использован, 0 - сигнал будет обнулён
        ''' </summary>
        Property Activities As Double()()

        ''' <summary>
        ''' Массив содержит слои и нейрон смещения (y)
        ''' y - номер слоя
        ''' (y) - значением является смещение. По-умолчанию 0, нет смещения
        ''' </summary>
        Property Biases As Double()

    End Interface

End Namespace
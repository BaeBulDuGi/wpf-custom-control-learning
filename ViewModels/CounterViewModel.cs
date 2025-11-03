using System;
using System.Windows;
using System.Windows.Input;

namespace WpfApp.ViewModels
{
    /// <summary>
    /// MVVM 패턴 예제 - 카운터 ViewModel
    /// View(XAML)와 Model(데이터)을 연결하는 중간 계층
    /// </summary>
    public class CounterViewModel : BaseViewModel
    {
        // ===== Private Fields =====
        private int _count;
        private string _message;
        private string _inputText;

        // ===== Properties (데이터 바인딩) =====

        /// <summary>
        /// 카운트 값 - UI에 바인딩됨
        /// 값이 변경되면 UI가 자동으로 업데이트됨
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                if (SetProperty(ref _count, value))
                {
                    // Count가 변경되면 메시지도 업데이트
                    UpdateMessage();
                }
            }
        }

        /// <summary>
        /// 표시 메시지 - UI에 바인딩됨
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        /// <summary>
        /// 입력 텍스트 - TextBox에 양방향 바인딩됨
        /// </summary>
        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        // ===== Commands (버튼 클릭 등) =====
        // ICommand: MVVM에서 사용자 액션을 처리하는 방법

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ShowInputCommand { get; }

        // ===== Constructor =====
        public CounterViewModel()
        {
            // 초기값 설정
            _count = 0;
            _message = "버튼을 클릭해보세요!";
            _inputText = "";

            // Command 초기화
            // RelayCommand: 람다 함수를 ICommand로 변환하는 헬퍼 클래스
            IncrementCommand = new RelayCommand(Increment, CanIncrement);
            DecrementCommand = new RelayCommand(Decrement, CanDecrement);
            ResetCommand = new RelayCommand(Reset);
            ShowInputCommand = new RelayCommand(ShowInput, CanShowInput);
        }

        // ===== Command 실행 메서드 =====

        private void Increment(object? parameter)
        {
            Count++;
        }

        private bool CanIncrement(object? parameter)
        {
            // Count가 100 미만일 때만 실행 가능
            return Count < 100;
        }

        private void Decrement(object? parameter)
        {
            Count--;
        }

        private bool CanDecrement(object? parameter)
        {
            // Count가 0 초과일 때만 실행 가능
            return Count > 0;
        }

        private void Reset(object? parameter)
        {
            Count = 0;
            InputText = "";
        }

        private void ShowInput(object? parameter)
        {
            if (!string.IsNullOrWhiteSpace(InputText))
            {
                Message = $"입력하신 내용: {InputText}";
            }
        }

        private bool CanShowInput(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(InputText);
        }

        // ===== Private Methods =====

        private void UpdateMessage()
        {
            if (Count == 0)
                Message = "0입니다. 증가 버튼을 눌러보세요!";
            else if (Count >= 100)
                Message = "최대값 100에 도달했습니다!";
            else if (Count % 10 == 0)
                Message = $"좋아요! 현재 {Count}입니다.";
            else
                Message = $"현재 카운트: {Count}";
        }
    }

    /// <summary>
    /// RelayCommand: ICommand를 쉽게 구현하기 위한 헬퍼 클래스
    /// 실제 프로젝트에서는 CommunityToolkit.Mvvm 같은 라이브러리 사용 추천
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}

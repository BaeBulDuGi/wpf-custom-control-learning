using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp.ViewModels
{
    /// <summary>
    /// MVVM 패턴의 기본 ViewModel
    /// INotifyPropertyChanged 구현으로 데이터 변경 시 UI 자동 업데이트
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        // ===== INotifyPropertyChanged 구현 =====
        // 이것이 MVVM의 핵심! 데이터가 변경되면 UI에 자동으로 알려줍니다

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 프로퍼티 변경 알림
        /// CallerMemberName: 호출한 프로퍼티 이름을 자동으로 가져옴
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 값 설정 헬퍼 메서드
        /// 값이 실제로 변경되었을 때만 알림
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

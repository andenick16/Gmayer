using System;
using System.Windows;
using System.Windows.Controls;

namespace GRTDappWpf.Models
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(string pageKey)
        {
            Page page = pageKey switch
            {
               // "Page1" => new Views.Page1(),
               // "Page2" => new Views.Page2(),
                _ => throw new ArgumentException($"Página '{pageKey}' não registrada.")
            };

            // Passa o DataContext do Frame para a nova página
            if (_frame.DataContext != null)
                page.DataContext = _frame.DataContext;

            _frame.Navigate(page);
        }
    }
}

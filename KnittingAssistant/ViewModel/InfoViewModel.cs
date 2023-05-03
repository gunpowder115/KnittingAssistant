using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace KnittingAssistant.ViewModel
{
    public class InfoViewModel : ViewModelBase
    {
        private const string VK_LINK = "https://vk.com/vlad_poroh";
        private const string GITHUB_LINK = "https://github.com/gunpowder115/KnittingAssistant";

        #region Dependency Properties

        private FlowDocument infoText;
        public FlowDocument InfoText
        {
            get { return infoText; }
            set
            {
                infoText = value;
                OnPropertyChanged("InfoText");
            }
        }

        #endregion

        #region Relay Commands

        private RelayCommand vkLinkClickCommand;
        public RelayCommand VkLinkClickCommand
        {
            get
            {
                return vkLinkClickCommand ??
                    (vkLinkClickCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo(VK_LINK) { UseShellExecute = true } );
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }));
            }
        }

        private RelayCommand githubLinkClickCommand;
        public RelayCommand GithubLinkClickCommand
        {
            get
            {
                return githubLinkClickCommand ??
                    (githubLinkClickCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo(GITHUB_LINK) { UseShellExecute = true } );
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }));
            }
        }

        #endregion
    }
}

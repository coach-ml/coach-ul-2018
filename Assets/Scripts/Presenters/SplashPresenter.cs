using Controllers;
using Models;
using ReactUnity;
using UnityEngine.UI;

namespace Presenters
{
    class SplashPresenter : Presenter<SplashController, SplashModel>
    {
        public InputField emailComponent;
        public InputField passwordComponent;

        protected override void Render(SplashModel state)
        {
        }

        public void Login()
        {
            Controller.SignIn(emailComponent.text, passwordComponent.text);
        }
    }
}

using System.Windows;
using testWpfProcedure.Repo.Data;
using testWpfProcedure.Repo.DataAccess;
using Microsoft.Practices.Unity;

namespace testWpfProcedure
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {


        protected override void OnStartup(StartupEventArgs e)
        {

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ISqlDataAccess, SqlDataAccess>();
            container.RegisterType<IUserData, UserData>();

         





        }





    }






}

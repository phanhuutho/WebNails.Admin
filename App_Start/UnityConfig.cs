using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using WebNails.Admin.Interfaces;
using WebNails.Admin.Repositories;

namespace WebNails.Admin
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<INailSocialRepository, NailSocialRepository>();
            container.RegisterType<INailCouponRepository, NailCouponRepository>();
            container.RegisterType<INailPricesRepository, NailPricesRepository>();
            container.RegisterType<INailAccountRepository, NailAccountRepository>();
            container.RegisterType<INailRepository, NailRepository>();
            container.RegisterType<ISocialRepository, SocialRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
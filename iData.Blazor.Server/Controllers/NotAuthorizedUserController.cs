using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iData.Blazor.Server.Controllers
{

    public partial class NotAuthorizedUserController : WindowController
    {
        public NotAuthorizedUserController()
        {
            InitializeComponent();
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var security = Application.Security;

            if (security.IsAuthenticated && security.User is ISecurityUserWithRoles user)
            {
                var objectSpace = Application.CreateObjectSpace(typeof(PermissionPolicyRole));
                var allRoles = objectSpace.GetObjects<PermissionPolicyRole>();

                var matchingRole = user.Roles.FirstOrDefault(userRole =>
                    allRoles.Any(globalRole => globalRole.Name == userRole.Name)
                );

                if (matchingRole is null)
                {
                    var navigationManager = (NavigationManager)Application.ServiceProvider.GetService(typeof(NavigationManager));
                    if (navigationManager != null)
                    {
                        navigationManager.NavigateTo("/not-authorized-user-view", true);
                        return;
                    }
                }
            }
        }
      
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}

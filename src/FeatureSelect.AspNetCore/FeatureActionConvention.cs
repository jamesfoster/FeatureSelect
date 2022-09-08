using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace FeatureSelect.AspNetCore;

internal class MvcFeatureConvention : IApplicationModelConvention
{
    private readonly FeatureSource source;

    public MvcFeatureConvention(FeatureSource source)
    {
        this.source = source;
    }

    public void Apply(ApplicationModel application)
    {
        var enabledControllers = application.Controllers.SelectMany(FilterController).ToList();

        application.Controllers.Clear();

        enabledControllers.ForEach(application.Controllers.Add);
    }

    private IEnumerable<ControllerModel> FilterController(ControllerModel controller)
    {
        var featureAttr = controller.ControllerType.GetCustomAttribute<IfFeatureAttribute>();

        if (featureAttr == null)
        {
            return new[] { controller };
        }

        IEnumerable<ControllerModel> included() => new[] { FilterActions(controller) };
        IEnumerable<ControllerModel> excluded() => Array.Empty<ControllerModel>();

        return featureAttr.Enabled
            ? source.GetFeature(featureAttr.Name).Execute(included, excluded)
            : source.GetFeature(featureAttr.Name).Execute(excluded, included);
    }

    private ControllerModel FilterActions(ControllerModel controller)
    {
        var enabledActions = controller.Actions.SelectMany(FilterAction).ToList();

        controller.Actions.Clear();

        enabledActions.ForEach(controller.Actions.Add);

        return controller;
    }

    private IEnumerable<ActionModel> FilterAction(ActionModel action)
    {
        var featureAttr = action.ActionMethod.GetCustomAttribute<IfFeatureAttribute>();

        if (featureAttr == null)
        {
            return new[] { action };
        }

        IEnumerable<ActionModel> included() => new[] { action };
        IEnumerable<ActionModel> excluded() => Array.Empty<ActionModel>();

        return featureAttr.Enabled
            ? source.GetFeature(featureAttr.Name).Execute(included, excluded)
            : source.GetFeature(featureAttr.Name).Execute(excluded, included);
    }
}

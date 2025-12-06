using HassWebView.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HassWebView.Core.Behaviors;

public class RemoteControlBehavior : Behavior<Page>
{
    private KeyService _keyService;

    protected override void OnAttachedTo(Page page)
    {
        base.OnAttachedTo(page);

        if (page is not IKeyHandler keyHandler)
        {
            // If the page doesn't implement IKeyHandler, there's nothing for this behavior to do.
            return;
        }

#if ANDROID || WINDOWS
        _keyService = page.Handler?.MauiContext?.Services.GetService<KeyService>();
#endif
        
        if (_keyService != null)
        {
            _keyService.Register(keyHandler);
        }
    }

    protected override void OnDetachingFrom(Page page)
    {
        if (_keyService != null)
        {
            _keyService.Unregister();
        }
        
        base.OnDetachingFrom(page);
    }
}

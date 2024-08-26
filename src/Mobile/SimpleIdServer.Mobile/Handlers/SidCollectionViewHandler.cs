#if ANDROID

using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Handlers.Items;

namespace SimpleIdServer.Mobile.Handlers;

public class SidCollectionViewHandler : CollectionViewHandler
{
    private IItemsLayout ItemsLayout {  get; set; }

    protected override IItemsLayout GetItemsLayout()
    {
        return ItemsLayout;
    }

    protected override void ConnectHandler(RecyclerView platformView)
    {
        base.ConnectHandler(platformView);
        ItemsLayout = VirtualView.ItemsLayout;
    }
}

#endif
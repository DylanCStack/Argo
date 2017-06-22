//This class has been altered from its origional state. Code added to @interface between empty comments. And added to @implementation from top to first empty comment. Code here ads functionality to detect a launch of the app with arguements passed via a custom URLScheme

/*============================================================================
Copyright (c) 2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
============================================================================*/

#import "UnityAppController.h"
#import "VuforiaRenderDelegate.h"


// Unity native rendering callback plugin mechanism is only supported
// from version 4.5 onwards
#if UNITY_VERSION>434

// Exported methods for native rendering callback
extern "C" void VuforiaSetGraphicsDevice(void* device, int deviceType, int eventType);
extern "C" void VuforiaRenderEvent(int marker);

#endif

// Controller to support native rendering callback
@interface VuforiaNativeRendererController : UnityAppController
{
}
- (void)shouldAttachRenderDelegate;
//
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions;

- (void) openURLAfterDelay:(NSURL*) url;

-(BOOL) application:(UIApplication *)application handleOpenURL:(NSURL *)url;

-(BOOL) application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
//
@end

@implementation VuforiaNativeRendererController
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];

    if ([launchOptions objectForKey:UIApplicationLaunchOptionsURLKey]) {
        NSURL *url = [launchOptions objectForKey:UIApplicationLaunchOptionsURLKey];

        [self performSelector:@selector(openURLAfterDelay:) withObject:url afterDelay:2];
    }

    return YES;
}

- (void) openURLAfterDelay:(NSURL*) url
{

    UnitySendMessage("LaunchManager", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
}

-(BOOL) application:(UIApplication *)application handleOpenURL:(NSURL *)url
{

    UnitySendMessage("LaunchManager", "OnOpenWithUrl", [[url absoluteString] UTF8String]);

    return YES;

}


-(BOOL) application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation

{
    UnitySendMessage("LaunchManager", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
    return YES;

}

///////

- (void)shouldAttachRenderDelegate
{
	self.renderDelegate = [[VuforiaRenderDelegate alloc] init];

// Unity native rendering callback plugin mechanism is only supported
// from version 4.5 onwards
#if UNITY_VERSION>434
	UnityRegisterRenderingPlugin(&VuforiaSetGraphicsDevice, &VuforiaRenderEvent);
#endif
}
@end


IMPL_APP_CONTROLLER_SUBCLASS(VuforiaNativeRendererController)

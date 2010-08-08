//
//  UIBinding.m
//  Unity-iPhone

#import "UIBinding.h"
#import "UnityNativeManager.h"


void _activateUIWithController( const char *controllerName )
{
	NSString *className = [NSString stringWithUTF8String:controllerName];
	
	[[UnityNativeManager sharedManager] showViewControllerWithName:className];
}


void _deactivateUI()
{
	[[UnityNativeManager sharedManager] hideViewControllerAndRestartUnity];
}
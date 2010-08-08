//
//  UnityNativeManager.h
//  Unity-iPhone
//
//  Created by Mike on 8/7/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>


@interface UnityNativeManager : NSObject
{
	UINavigationController *_navigationControler;
	NSString *_animationType;
	NSString *_animationSubtype;
	CAMediaTimingFunction *_animationTimingFunction;
	CFTimeInterval _animationDuration;
}
@property (nonatomic, retain) UINavigationController *navigationControler;
@property (nonatomic, retain) NSString *animationType; // kCATransitionFade, kCATransitionMoveIn, kCATransitionPush, kCATransitionMoveIn
@property (nonatomic, retain) NSString *animationSubtype; // kCATransitionFromRight, kCATransitionFromLeft, kCATransitionFromTop, kCATransitionFromBottom
@property (nonatomic, retain) CAMediaTimingFunction *animationTimingFunction;
@property (nonatomic, assign) CFTimeInterval animationDuration;


+ (UnityNativeManager*)sharedManager;

// Sets the animation that will be used for showing/hiding native views
- (void)setAnimationType:(NSString*)animationType subtype:(NSString*)animationSubtype;

// Shows a viewController with the given class name.
- (void)showViewControllerWithName:(NSString*)name;

// Hides the native UI and returns control to Unity
- (void)hideViewControllerAndRestartUnity;

@end

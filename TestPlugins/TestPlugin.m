//
//  TestPlugin.m
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/12/17.
//
//

#import "TestPlugin.h"



@implementation TestPlugin : NSObject

- (NSString*)testMethod {
    NSLog(@"Native plugin is working!");
    return @"native plugin is working";
}

@end

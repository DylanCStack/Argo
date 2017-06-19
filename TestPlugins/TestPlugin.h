//
//  TestPlugin.h
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/12/17.
//
//

#import <Foundation/Foundation.h>

NSString* _testMethod(); // THIS is the method that Unity looks for.

@interface TestPlugin : NSObject
- (NSString*)testMethod; // Unity can't see this one!
@end

//
//  VideoPicker.m
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/16/17.
//
//

#import <UIKit/UIKit.h>
#import <MobileCoreServices/MobileCoreServices.h>
#import <AVFoundation/AVFoundation.h>
#if UNITY_VERSION <= 434
#import "iPhone_View.h"
#endif
char video_url_path[1024];
static inline CGFloat RadiansToDegrees(CGFloat radians) {
    return radians * 180 / M_PI;
};


@interface NonRotatingUIImagePickerController : UIImagePickerController
@end
@implementation NonRotatingUIImagePickerController
- (NSUInteger)supportedInterfaceOrientations{
    return UIInterfaceOrientationMaskLandscape;
}
@end
//-----------------------------------------------------------------
@interface APLViewController : UIViewController <UINavigationControllerDelegate, UIImagePickerControllerDelegate>{
    UIImagePickerController *imagePickerController;
@public
    const char *callback_game_object_name ;
    const char *callback_function_name ;
}
@end
@implementation APLViewController
- (void)viewDidLoad
{
    [super viewDidLoad];
    [self showImagePickerForSourceType:UIImagePickerControllerSourceTypePhotoLibrary];
}
- (void)showImagePickerForSourceType:(UIImagePickerControllerSourceType)sourceType
{
    imagePickerController = [[UIImagePickerController alloc] init];
    imagePickerController.modalPresentationStyle = UIModalPresentationCurrentContext;
    imagePickerController.sourceType = sourceType;
    imagePickerController.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie,nil];
    imagePickerController.delegate = self;
    [self.view addSubview:imagePickerController.view];
}
#pragma mark - UIImagePickerControllerDelegate
// This method is called when an image has been chosen from the library or taken from the camera.
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    NSString *type = [info objectForKey:UIImagePickerControllerMediaType];
    //NSLog(@"type=%@",type);
    if ([type isEqualToString:(NSString *)kUTTypeVideo] ||
        [type isEqualToString:(NSString *)kUTTypeMovie])
    {// movie != video
        NSURL *urlvideo = [info objectForKey:UIImagePickerControllerMediaURL];
        NSLog(@"%@", urlvideo);
        NSString *urlString = [urlvideo absoluteString];
        
        AVAsset *videoAsset = [AVAsset assetWithURL:urlvideo];
        CGSize trackDimensions = {
            .width = 0.0,
            .height = 0.0,
        };
        
        trackDimensions = [videoAsset naturalSize];
        
        float width = trackDimensions.width;
        float height = trackDimensions.height;
        float aspectRatio = height/width;
        NSNumber *aspectRatioNumber = @(aspectRatio);
        
        NSNumberFormatter *nf = [NSNumberFormatter new];
        nf.numberStyle = NSNumberFormatterDecimalStyle;
        
        NSString *aspectRatioString = [nf stringFromNumber:aspectRatioNumber];
        NSLog(aspectRatioString);
        
        NSString *totalVideoString = [[urlString stringByAppendingString:@"|"]stringByAppendingString:aspectRatioString];
        NSLog(totalVideoString);
        
        const char* cp = [totalVideoString UTF8String];
        strcpy(video_url_path, cp);
    }
    [self dismissViewControllerAnimated:YES completion:NULL];
    // UnitySendMessage("GameObject", "VideoPicked", video_url_path);
    UnitySendMessage(callback_game_object_name, callback_function_name, video_url_path);
}
- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    const char *obj = "TestObject";
    const char *method = "PickerDidCancel";
    const char *msg = "Cancel";
    UnitySendMessage(obj, method, msg);
    [self dismissViewControllerAnimated:YES completion:NULL];
}
@end
extern "C" {
    void OpenVideoPicker(const char *game_object_name, const char *function_name) {
        if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary]) {
            // APLViewController
            UIViewController* parent = UnityGetGLViewController();
            APLViewController *uvc = [[APLViewController alloc] init];
            uvc->callback_game_object_name = strdup(game_object_name) ;
            uvc->callback_function_name = strdup(function_name) ;
            [parent presentViewController:uvc animated:YES completion:nil];
        }
    }
}

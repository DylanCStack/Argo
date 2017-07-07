//
//  VideoPicker.m
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/16/17.
//
//


//import relevant frameworks - make sure to add these to your libraries in build phases
#import <UIKit/UIKit.h>
#import <MobileCoreServices/MobileCoreServices.h>
#import <AVFoundation/AVFoundation.h>
#if UNITY_VERSION <= 434
#import "iPhone_View.h"
#endif

//global variable to hold the video path
char video_url_path[1024];

//helper function for determining video angles
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
@interface APLViewController : UIViewController <UINavigationControllerDelegate, UIImagePickerControllerDelegate>{ //set the delegate as an imagePickerController
    UIImagePickerController *imagePickerController;
@public
    const char *callback_game_object_name ; //values set by unity
    const char *callback_function_name ;  //values set by unity
}
@end
@implementation APLViewController
- (void)viewDidLoad
{
  //on the super loading, show the imagePickerController
    [super viewDidLoad];
    [self showImagePickerForSourceType:UIImagePickerControllerSourceTypePhotoLibrary];
}
- (void)showImagePickerForSourceType:(UIImagePickerControllerSourceType)sourceType
{//shows image picker controller and sets delegate as this class
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
    if ([type isEqualToString:(NSString *)kUTTypeVideo] ||
        [type isEqualToString:(NSString *)kUTTypeMovie])
    {// movie != video
        NSURL *urlvideo = [info objectForKey:UIImagePickerControllerMediaURL];
        NSLog(@"%@", urlvideo);
        NSString *urlString = [urlvideo absoluteString];
        AVAsset *videoAsset = [AVAsset assetWithURL:urlvideo];

        CGFloat orgWidth;
        CGFloat orgHeight;

        //readout the size of video
        AVAssetTrack *vT = nil;
        if ([[videoAsset tracksWithMediaType:AVMediaTypeVideo] count] != 0)
        {
            vT = [[videoAsset tracksWithMediaType:AVMediaTypeVideo] objectAtIndex:0];
        }
        if (vT != nil)
        {
            orgWidth = vT.naturalSize.width;
            orgHeight = vT.naturalSize.height;
        }
        NSString *videoOrientation;
        //check the orientation
        CGAffineTransform txfb = [vT preferredTransform];
        if ((orgWidth == txfb.tx && orgHeight == txfb.ty)|(txfb.tx == 0 && txfb.ty == 0))
        {
            videoOrientation = @"landscape";
            NSLog(@"Landscape");
        }
        else
        {
            videoOrientation = @"portrait";
            NSLog(@"Portrait");
        }

        //calculate the aspect ratio
        float aspectRatio = orgHeight/orgWidth;
        NSNumber *aspectRatioNumber = @(aspectRatio);
        NSNumberFormatter *nf = [NSNumberFormatter new];
        nf.numberStyle = NSNumberFormatterDecimalStyle;

        NSString *aspectRatioString = [nf stringFromNumber:aspectRatioNumber];
        NSLog(@"%@", aspectRatioString);

        // append all info in to a string - unity can only recieve one string at a time
        NSString *totalVideoString = [[[[urlString stringByAppendingString:@"|"]stringByAppendingString:aspectRatioString]stringByAppendingString:@"|"]stringByAppendingString:videoOrientation];
        NSLog(@"%@", totalVideoString);

        //unity send message requires the char format
        const char* cp = [totalVideoString UTF8String];
        strcpy(video_url_path, cp);
    }
    [self dismissViewControllerAnimated:YES completion:NULL];
    // UnitySendMessage("GameObject", "VideoPicked", video_url_path);
    UnitySendMessage(callback_game_object_name, callback_function_name, video_url_path);
}
- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
  //on cancel, send message to unity to let it know
    const char *obj = "TestObject";
    const char *method = "PickerDidCancel";
    const char *msg = "Cancel";
    UnitySendMessage(obj, method, msg);
    [self dismissViewControllerAnimated:YES completion:NULL];
}
@end

//this is the code that is visible to unity - this function is imported and called in the qrscanner3 script
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

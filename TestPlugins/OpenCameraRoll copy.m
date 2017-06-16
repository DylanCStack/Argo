//
//  OpenCameraRoll.m
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/13/17.
//
//

#import "OpenCameraRoll.h"

void _chooseExisting()
{
    OpenCameraRoll *newCameraRoll = [OpenCameraRoll new];
    UIViewController *top = [UIApplication sharedApplication].keyWindow.rootViewController;
    UIImagePickerController *picker2 =  [[UIImagePickerController alloc] init];
    picker2.delegate = newCameraRoll;
    [picker2 setSourceType:(UIImagePickerControllerSourceTypePhotoLibrary)];
    NSLog(@"hello from choose existing");
    NSLog(picker2.debugDescription);
    NSLog(newCameraRoll.debugDescription);
    [top presentViewController:picker2 animated:YES  completion:NULL];
}

@interface OpenCameraRoll ()

@end

@implementation OpenCameraRoll

- (IBAction)TakePhoto {
    picker =  [[UIImagePickerController alloc] init];
    picker.delegate = self;
    [picker setSourceType:(UIImagePickerControllerSourceTypeCamera)];
    [self presentViewController:picker animated:YES  completion:NULL];
    
}

- (IBAction)ChooseExisting {
    UIViewController *top = [UIApplication sharedApplication].keyWindow.rootViewController;
    picker2 =  [[UIImagePickerController alloc] init];
    picker2.delegate = self;
    [picker2 setSourceType:(UIImagePickerControllerSourceTypePhotoLibrary)];
    NSLog(@"hello from choose existing");
    [top presentViewController:picker2 animated:YES  completion:NULL];
    
}


//-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
//    NSLog(@"hello");
//    image = [info objectForKey:UIImagePickerControllerOriginalImage];
//    NSLog(@"hello2");
//    NSData *imageData = UIImagePNGRepresentation(image);
//    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
//    NSString *documentsDirectory = [paths objectAtIndex:0];
//    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:@"Test.jpg"]; //Add the file name
//    NSLog(@"filePath %@",filePath);
//    [imageData writeToFile:filePath atomically:YES];
//    const char c = [filePath UTF8String];
//    UnitySendMessage("ComposeButton", "recieveImagePath", "hola");
//    [self dismissViewControllerAnimated:YES completion:NULL];
//}

-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
    NSLog(@"hello from image picker controller (thank the lord)");
    [self dismissViewControllerAnimated:YES completion:NULL];
    
}

- (char*) returnImagePath {
    return imagePath;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    NSLog(@"hello from cancel");
    [self dismissViewControllerAnimated:YES completion:NULL];
}



- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
}


- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (UIWindow*) getTopApplicationWindow
{
    UIApplication* clientApp = [UIApplication sharedApplication];
    NSArray* windows = [clientApp windows];
    UIWindow* topWindow = nil;
    if (windows && [windows count] > 0)
        topWindow = [[clientApp windows] objectAtIndex:0];
    return topWindow;
}

- (id) GetSelf {
    return self;
}


@end

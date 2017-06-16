//
//  OpenCameraRoll.h
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/13/17.
//
//

#import <UIKit/UIKit.h>
#import <stdlib.h>
#import <Photos/Photos.h>
#import <PhotosUI/PhotosUI.h>

@interface OpenCameraRoll : UIViewController <UIImagePickerControllerDelegate, UINavigationControllerDelegate> {
    UIImagePickerController *picker;
    UIImagePickerController *picker2;
    UIImage *image;
    char *imagePath;
    
    IBOutlet UIImageView *imageView;
}

- (IBAction)TakePhoto;
- (IBAction)ChooseExisting;
- (char*) returnImagePath;
- (id) GetSelf;

@end

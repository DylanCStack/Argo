//
//  ContactPicker.mm
//  Unity-iPhone
//
//  Created by Clayton Collins on 6/26/17.
//
//

#import <MobileCoreServices/MobileCoreServices.h>
#import <UIKit/UIKit.h>
#import <ContactsUI/ContactsUI.h>
#import <Contacts/Contacts.h>
#if UNITY_VERSION <= 434
#import "iPhone_View.h"
#endif
char contact_name[1024];
@interface NonRotatingUIContactPickerController: CNContactPickerViewController
@end

@interface APLViewController2: UIViewController <UINavigationControllerDelegate, CNContactPickerDelegate> {
    CNContactPickerViewController *contactPickerController;
@public
    const char *callback_game_object_name2;
    const char *callback_function_name2;
}
@end
@implementation APLViewController2
- (void)viewDidLoad
{
    [super viewDidLoad];
    [self showContactPicker];
}

- (void)showContactPicker
{
    contactPickerController = [[CNContactPickerViewController alloc] init];
    contactPickerController.delegate = self;
    [self.view addSubview:contactPickerController.view];
}

- (void)getContacts
{
    CNContactStore *store = [[CNContactStore alloc] init];
    [store requestAccessForEntityType:CNEntityTypeContacts completionHandler:^(BOOL granted, NSError * _Nullable error) {
        if (granted == YES) {
            //keys with fetching properties
            NSArray *keys = @[CNContactFamilyNameKey, CNContactGivenNameKey, CNContactPhoneNumbersKey, CNContactImageDataKey];
            CNContactFetchRequest *request = [[CNContactFetchRequest alloc] initWithKeysToFetch:keys];
            NSError *error;
            BOOL success = [store enumerateContactsWithFetchRequest:request error:&error usingBlock:^(CNContact * __nonnull contact, BOOL * __nonnull stop) {
                if (error) {
                    NSLog(@"error fetching contacts %@", error);
                } else {
                    // copy data to my custom Contact class.
                    NSString *firstName = contact.givenName;
                    NSString *lastName = contact.familyName;
                    if ([contact.phoneNumbers count]) {
                        NSString *phoneNumber = [contact.phoneNumbers objectAtIndex:0].value.stringValue;
                        char c[1024];
                        strcpy(c, [[[[[firstName stringByAppendingString:@" "] stringByAppendingString:lastName] stringByAppendingString:@"|"]stringByAppendingString:phoneNumber] UTF8String]);
                        UnitySendMessage(callback_game_object_name2, callback_function_name2, c);
                    }
                    // etc.
                }
            }];
        }        
    }];
}
#pragma mark - CNContactPickerDelegate

- (void)conactPickerController:(CNContactPickerViewController *)picker didSelectContact:(CNContact *)contact
{
    [self dismissViewControllerAnimated:YES completion:nil];
    const char* cp = [contact.givenName UTF8String];
    strcpy(contact_name, cp);
    UnitySendMessage(callback_game_object_name2, callback_function_name2, contact_name);
}

- (void)contactPickerControllerDidCancel:(CNContactPickerViewController *)picker
{
    [self dismissViewControllerAnimated:YES completion:NULL];
}

@end

extern "C" {
    void OpenContactPicker(const char *game_object_name, const char *function_name) {
        UIViewController* parent = UnityGetGLViewController();
        APLViewController2 *uvc = [[APLViewController2 alloc] init];
        uvc->callback_game_object_name2 = strdup(game_object_name) ;
        uvc->callback_function_name2 = strdup(function_name) ;
        [uvc getContacts];
    }
}

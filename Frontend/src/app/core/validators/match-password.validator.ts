import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const matchPasswordValidator: ValidatorFn = (
    group: AbstractControl
): ValidationErrors | null => {
    const password = group.get('password')?.value as string | null;
    const confirmPassword = group.get('confirmPassword')?.value as string | null;

    if (!password || !confirmPassword) return null;

    return password === confirmPassword
        ? null
        : {
              passwordDontMatch: {
                  valid: false,
                  message: 'Passwords do not match.',
              },
          };
};

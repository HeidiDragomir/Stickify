import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const passwordValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  // Take the value from input
  const value = control.value?.trim();

  if (!value) return null;

  // Regex for password strength
  const strongPasswordPattern = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d\S]{8,}$/;

  const isStrong = strongPasswordPattern.test(value);

  return isStrong
    ? null
    : {
        passwordStrength: {
          valid: false,
          message: 'Password does not meet complexity requirements.',
        },
      };
};

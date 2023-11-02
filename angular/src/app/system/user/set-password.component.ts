import { takeUntil } from 'rxjs';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { OnInit, OnDestroy, EventEmitter } from '@angular/core';
import { UserService } from '@proxy/system/users';
import { AuthService } from 'src/app/shared/services/auth.service';
import { RoleDto } from '@proxy/system/roles';
@Component({
  templateUrl: 'set-password.component.html',
})
export class SetPasswordComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public closeBtnName: string;
  selectedEntity = {} as RoleDto;

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    public authService: AuthService,
    public config: DynamicDialogConfig,
    public ref: DynamicDialogRef
  ) {}

  ngOnDestroy(): void {
    //Called once, before the instance is destroyed.
    //Add 'implements OnDestroy' to the class.
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    this.buildForm();
    this.saveBtnName = 'Cập nhật';
    this.closeBtnName = 'Hủy';
  }

  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    passsword: [
      { type: 'required', message: 'Bạn phải nhập mật khẩu' },
      {
        type: 'pattern',
        message: 'Mật khẩu ít nhất 8 ký tự, ít nhất 1 số, 1 ký tự đặc biệt, và một chữ hoa',
      },
    ],
    confirmPassword: [{ type: 'required', message: 'Xác nhận mật khẩu không đúng' }],
  };

  saveChange() {
    this.toggleBlockUI(true);

    this.saveData();
  }

  private saveData() {
    this.userService
      .setPassword(this.config.data.id, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subcribe(() => {
        this.toggleBlockUI(false);
        this.ref.close();
      });
  }

  buildForm() {
    this.form = this.fb.group({
      newPassword: new FormControl(
        null,
        Validators.compose[
          (Validators.required,
          Validators.pattern(
            '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$'
          ))
        ]
      ),
      confirmPassword: new FormControl(null),
    });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
}

export const passwordMatchingValidatior: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmNewPassword');

  return password?.value === confirmPassword?.value ? null : { notmatched: true };
};

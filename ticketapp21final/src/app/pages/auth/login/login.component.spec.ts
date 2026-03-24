import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent, RouterTestingModule, HttpClientTestingModule]
    }).compileComponents();
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());
  it('showPwd defaults false', () => expect(component.showPwd()).toBeFalse());
  it('loading defaults false', () => expect(component.loading()).toBeFalse());
  it('form starts empty', () => {
    expect(component.form.userName).toBe('');
    expect(component.form.password).toBe('');
  });
  it('togglePwd flips showPwd', () => {
    component.togglePwd();
    expect(component.showPwd()).toBeTrue();
    component.togglePwd();
    expect(component.showPwd()).toBeFalse();
  });
  it('does not call login when already loading', () => {
    component.loading.set(true);
    const spy = spyOn(component.auth, 'login').and.callThrough();
    component.onSubmit();
    expect(spy).not.toHaveBeenCalled();
  });
});

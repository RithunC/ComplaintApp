import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RegisterComponent } from './register.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterComponent, RouterTestingModule, HttpClientTestingModule]
    }).compileComponents();
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('roles pre-filled with defaults on load', () => {
    expect(component.roles().length).toBeGreaterThan(0);
    expect(component.roles().some(r => r.name === 'Employee')).toBeTrue();
  });

  it('departments pre-filled with defaults', () => {
    expect(component.departments().length).toBeGreaterThan(0);
    expect(component.departments()[0].name).toBe('IT Support');
  });

  it('shows error toast when roleName empty', () => {
    const spy = spyOn(component.toast, 'error');
    component.form.userName     = 'test';
    component.form.email        = 'test@test.com';
    component.form.displayName  = 'Test';
    component.form.password     = '123456';
    component.form.roleName     = '';
    component.onSubmit();
    expect(spy).toHaveBeenCalledWith('Please select a role.');
  });

  it('togglePwd flips showPwd', () => {
    expect(component.showPwd()).toBeFalse();
    component.togglePwd();
    expect(component.showPwd()).toBeTrue();
  });
});

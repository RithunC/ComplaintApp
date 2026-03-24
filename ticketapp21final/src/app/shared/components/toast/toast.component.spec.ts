import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ToastComponent } from './toast.component';
describe('ToastComponent', () => {
  let f: ComponentFixture<ToastComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[ToastComponent] }).compileComponents(); f = TestBed.createComponent(ToastComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('icon returns check for success', () => expect(f.componentInstance.icon('success')).toBe('bi-check-circle-fill'));
  it('icon returns x for error', () => expect(f.componentInstance.icon('error')).toBe('bi-x-circle-fill'));
});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SpinnerComponent } from './spinner.component';
describe('SpinnerComponent', () => {
  let f: ComponentFixture<SpinnerComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[SpinnerComponent] }).compileComponents(); f = TestBed.createComponent(SpinnerComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('overlay hidden when not loading', () => expect(f.nativeElement.querySelector('.overlay')).toBeNull());
});

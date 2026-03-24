import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfileComponent } from './profile.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('ProfileComponent', () => {
  let f: ComponentFixture<ProfileComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ProfileComponent, RouterTestingModule, HttpClientTestingModule] }).compileComponents();
    f = TestBed.createComponent(ProfileComponent);
    f.detectChanges();
  });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('editing starts false', () => expect(f.componentInstance.editing()).toBeFalse());
  it('initials returns U with no profile', () => expect(f.componentInstance.initials()).toBe('U'));
});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NavbarComponent } from './navbar.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('NavbarComponent', () => {
  let f: ComponentFixture<NavbarComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[NavbarComponent, RouterTestingModule, HttpClientTestingModule] }).compileComponents(); f = TestBed.createComponent(NavbarComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('initials returns U when no user', () => expect(f.componentInstance.initials()).toBe('U'));
});

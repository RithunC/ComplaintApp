import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTicketComponent } from './create-ticket.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('CreateTicketComponent', () => {
  let f: ComponentFixture<CreateTicketComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [CreateTicketComponent, RouterTestingModule, HttpClientTestingModule] }).compileComponents();
    f = TestBed.createComponent(CreateTicketComponent);
    f.detectChanges();
  });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('title empty initially', () => expect(f.componentInstance.form.title).toBe(''));
  it('priorityId zero initially', () => expect(f.componentInstance.form.priorityId).toBe(0));
  it('titleLen reflects form.title', () => { f.componentInstance.form.title = 'Hello'; expect(f.componentInstance.titleLen).toBe(5); });
  it('does not submit when loading', () => {
    f.componentInstance.loading.set(true);
    const spy = spyOn(f.componentInstance.ts, 'create').and.callThrough();
    f.componentInstance.onSubmit();
    expect(spy).not.toHaveBeenCalled();
  });
});

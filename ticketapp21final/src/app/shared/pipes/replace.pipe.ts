import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'replace', standalone: true })
export class ReplacePipe implements PipeTransform {
  transform(value: string, search: string, rep: string): string {
    return value ? value.split(search).join(rep) : '';
  }
}

import assert from 'node:assert/strict';
import {parseDispatchQr,DispatchQrFormatError} from '../src/app/features/dispatches/qr/dispatch-qr.parser.ts';

function rejects(raw,label){
 assert.throws(()=>parseDispatchQr(raw),DispatchQrFormatError,label);
}

const legacy='tcKimlikNo:12345678901,dogumTarihi:03.07.1990,sevkEvrakTarihi:30.10.2026,'+
 'sevkEvrakSayisi:123/45,muayeneSebebi:01234567-89ab-4cde-8f01-23456789abcd,'+
 'sevkKapsamiHastane:aaaaaaaa-bbbb-4ccc-8ddd-eeeeeeeeeeee,'+
 'alimId:22,kacinciIslem:3,telefonNo:5012345678';
const data=parseDispatchQr(legacy);
assert.equal(data.tcKimlikNo,'12345678901');
assert.equal(data.dogumTarihi,'1990-07-03');
assert.equal(data.sevkEvrakTarihi,'2026-10-30');
assert.equal(data.sevkEvrakSayisi,'123/45');
assert.equal(data.alimId,22);
assert.equal(data.kacinciIslem,3);
assert.equal(data.telefonNo,'5012345678');

const quoted=parseDispatchQr('"yabanciKimlikNo":"12345678901","dogumTarihi":"29.02.2024"');
assert.equal(quoted.yabanciKimlikNo,'12345678901');
assert.equal(quoted.dogumTarihi,'2024-02-29');

assert.equal(parseDispatchQr('{"sevkEvrakSayisi":"ABC/31","ignored":"sensitive"}').sevkEvrakSayisi,'ABC/31');
assert.equal(Object.keys(parseDispatchQr('{"sevkEvrakSayisi":"ABC/31","ignored":"sensitive"}')).length,1);

rejects('dogumTarihi:31.02.2026','31 Şubat hatalı olmalı');
rejects('telefonNo:12345','Telefon 10 haneli olmalı');
rejects('tcKimlikNo:12345678901,tcKimlikNo:12345678901','Yinelenen alan hatası');
rejects('muayeneSebebi:not-a-guid','GUID doğrulaması');
rejects('unexpectedField:anything','Bilinmeyen alan kabul edilmez');
rejects('','Boş QR reddi');
rejects('tcKimlikNo:12345678901,yabanciKimlikNo:12345678901','Tek kimlik türü');
console.log('PASS: Sevk QR legacy/JSON, tarih, kimlik, telefon, GUID ve allowlist regresyonları');

using System;

namespace PKHeX.Core;

public static class StringFont8b
{
    // Each Unicode font has a table that maps supported Unicode codepoints to glyphs in the font (called cmap in OpenType fonts).
    // If a codepoint is not present in this table, the games will display a fallback character instead (a question mark or a space).
    // Since we only care if a codepoint is defined, we can store this data just by using bitflags in a byte array for O(1) lookup.

    // LGP/E, SW/SH, PLA: bin/font
    // BD/SP: StreamingAssets/AssetAssistant/Dpr/font
    // S/V: arc/appli/font/bin
    // For KOR/CHS/CHT, BD/SP uses a bundled copy of the Switch system font
    
    public static ReadOnlySpan<char> DefinedLiberationSans =>
    [
        '\u0020', '\u0021', '\u0022', '\u0023', '\u0024', '\u0025', '\u0026', '\u0027', '\u0028', '\u0029', '\u002A', '\u002B', '\u002C', '\u002D', '\u002E', '\u002F',
        '\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', '\u0036', '\u0037', '\u0038', '\u0039', '\u003A', '\u003B', '\u003C', '\u003D', '\u003E', '\u003F',
        '\u0040', '\u0041', '\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047', '\u0048', '\u0049', '\u004A', '\u004B', '\u004C', '\u004D', '\u004E', '\u004F',
        '\u0050', '\u0051', '\u0052', '\u0053', '\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059', '\u005A', '\u005B', '\u005C', '\u005D', '\u005E', '\u005F',
        '\u0060', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065', '\u0066', '\u0067', '\u0068', '\u0069', '\u006A', '\u006B', '\u006C', '\u006D', '\u006E', '\u006F',
        '\u0070', '\u0071', '\u0072', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077', '\u0078', '\u0079', '\u007A', '\u007B', '\u007C', '\u007D', '\u007E',
        '\u00A0', '\u00A1', '\u00A2', '\u00A3', '\u00A4', '\u00A5', '\u00A6', '\u00A7', '\u00A8', '\u00A9', '\u00AA', '\u00AB', '\u00AC', '\u00AD', '\u00AE', '\u00AF',
        '\u00B0', '\u00B1', '\u00B2', '\u00B3', '\u00B4', '\u00B5', '\u00B6', '\u00B7', '\u00B8', '\u00B9', '\u00BA', '\u00BB', '\u00BC', '\u00BD', '\u00BE', '\u00BF',
        '\u00C0', '\u00C1', '\u00C2', '\u00C3', '\u00C4', '\u00C5', '\u00C6', '\u00C7', '\u00C8', '\u00C9', '\u00CA', '\u00CB', '\u00CC', '\u00CD', '\u00CE', '\u00CF',
        '\u00D0', '\u00D1', '\u00D2', '\u00D3', '\u00D4', '\u00D5', '\u00D6', '\u00D7', '\u00D8', '\u00D9', '\u00DA', '\u00DB', '\u00DC', '\u00DD', '\u00DE', '\u00DF',
        '\u00E0', '\u00E1', '\u00E2', '\u00E3', '\u00E4', '\u00E5', '\u00E6', '\u00E7', '\u00E8', '\u00E9', '\u00EA', '\u00EB', '\u00EC', '\u00ED', '\u00EE', '\u00EF',
        '\u00F0', '\u00F1', '\u00F2', '\u00F3', '\u00F4', '\u00F5', '\u00F6', '\u00F7', '\u00F8', '\u00F9', '\u00FA', '\u00FB', '\u00FC', '\u00FD', '\u00FE', '\u00FF',
        '\u0100', '\u0101', '\u0102', '\u0103', '\u0104', '\u0105', '\u0106', '\u0107', '\u0108', '\u0109', '\u010A', '\u010B', '\u010C', '\u010D', '\u010E', '\u010F',
        '\u0110', '\u0111', '\u0112', '\u0113', '\u0114', '\u0115', '\u0116', '\u0117', '\u0118', '\u0119', '\u011A', '\u011B', '\u011C', '\u011D', '\u011E', '\u011F',
        '\u0120', '\u0121', '\u0122', '\u0123', '\u0124', '\u0125', '\u0126', '\u0127', '\u0128', '\u0129', '\u012A', '\u012B', '\u012C', '\u012D', '\u012E', '\u012F',
        '\u0130', '\u0131', '\u0132', '\u0133', '\u0134', '\u0135', '\u0136', '\u0137', '\u0138', '\u0139', '\u013A', '\u013B', '\u013C', '\u013D', '\u013E', '\u013F',
        '\u0140', '\u0141', '\u0142', '\u0143', '\u0144', '\u0145', '\u0146', '\u0147', '\u0148', '\u0149', '\u014A', '\u014B', '\u014C', '\u014D', '\u014E', '\u014F',
        '\u0150', '\u0151', '\u0152', '\u0153', '\u0154', '\u0155', '\u0156', '\u0157', '\u0158', '\u0159', '\u015A', '\u015B', '\u015C', '\u015D', '\u015E', '\u015F',
        '\u0160', '\u0161', '\u0162', '\u0163', '\u0164', '\u0165', '\u0166', '\u0167', '\u0168', '\u0169', '\u016A', '\u016B', '\u016C', '\u016D', '\u016E', '\u016F',
        '\u0170', '\u0171', '\u0172', '\u0173', '\u0174', '\u0175', '\u0176', '\u0177', '\u0178', '\u0179', '\u017A', '\u017B', '\u017C', '\u017D', '\u017E', '\u017F',
        '\u0180', '\u0181', '\u0182', '\u0183', '\u0184', '\u0185', '\u0186', '\u0187', '\u0188', '\u0189', '\u018A', '\u018B', '\u018C', '\u018D', '\u018E', '\u018F',
        '\u0190', '\u0191', '\u0192', '\u0193', '\u0194', '\u0195', '\u0196', '\u0197', '\u0198', '\u0199', '\u019A', '\u019B', '\u019C', '\u019D', '\u019E', '\u019F',
        '\u01A0', '\u01A1', '\u01A2', '\u01A3', '\u01A4', '\u01A5', '\u01A6', '\u01A7', '\u01A8', '\u01A9', '\u01AA', '\u01AB', '\u01AC', '\u01AD', '\u01AE', '\u01AF',
        '\u01B0', '\u01B1', '\u01B2', '\u01B3', '\u01B4', '\u01B5', '\u01B6', '\u01B7', '\u01B8', '\u01B9', '\u01BA', '\u01BB', '\u01BC', '\u01BD', '\u01BE', '\u01BF',
        '\u01C0', '\u01C1', '\u01C2', '\u01C3', '\u01C4', '\u01C5', '\u01C6', '\u01C7', '\u01C8', '\u01C9', '\u01CA', '\u01CB', '\u01CC', '\u01CD', '\u01CE', '\u01CF',
        '\u01D0', '\u01D1', '\u01D2', '\u01D3', '\u01D4', '\u01D5', '\u01D6', '\u01D7', '\u01D8', '\u01D9', '\u01DA', '\u01DB', '\u01DC', '\u01DD', '\u01DE', '\u01DF',
        '\u01E0', '\u01E1', '\u01E2', '\u01E3', '\u01E4', '\u01E5', '\u01E6', '\u01E7', '\u01E8', '\u01E9', '\u01EA', '\u01EB', '\u01EC', '\u01ED', '\u01EE', '\u01EF',
        '\u01F0', '\u01F1', '\u01F2', '\u01F3', '\u01F4', '\u01F5', '\u01F6', '\u01F7', '\u01F8', '\u01F9', '\u01FA', '\u01FB', '\u01FC', '\u01FD', '\u01FE', '\u01FF',
        '\u0200', '\u0201', '\u0202', '\u0203', '\u0204', '\u0205', '\u0206', '\u0207', '\u0208', '\u0209', '\u020A', '\u020B', '\u020C', '\u020D', '\u020E', '\u020F',
        '\u0210', '\u0211', '\u0212', '\u0213', '\u0214', '\u0215', '\u0216', '\u0217', '\u0218', '\u0219', '\u021A', '\u021B', '\u021C', '\u021D', '\u021E', '\u021F',
        '\u0220', '\u0221', '\u0222', '\u0223', '\u0224', '\u0225', '\u0226', '\u0227', '\u0228', '\u0229', '\u022A', '\u022B', '\u022C', '\u022D', '\u022E', '\u022F',
        '\u0230', '\u0231', '\u0232', '\u0233', '\u0234', '\u0235', '\u0236', '\u0237', '\u0238', '\u0239', '\u023A', '\u023B', '\u023C', '\u023D', '\u023E', '\u023F',
        '\u0240', '\u0241', '\u0242', '\u0243', '\u0244', '\u0245', '\u0246', '\u0247', '\u0248', '\u0249', '\u024A', '\u024B', '\u024C', '\u024D', '\u024E', '\u024F',
        '\u0250', '\u0251', '\u0252', '\u0253', '\u0254', '\u0255', '\u0256', '\u0257', '\u0258', '\u0259', '\u025A', '\u025B', '\u025C', '\u025D', '\u025E', '\u025F',
        '\u0260', '\u0261', '\u0262', '\u0263', '\u0264', '\u0265', '\u0266', '\u0267', '\u0268', '\u0269', '\u026A', '\u026B', '\u026C', '\u026D', '\u026E', '\u026F',
        '\u0270', '\u0271', '\u0272', '\u0273', '\u0274', '\u0275', '\u0276', '\u0277', '\u0278', '\u0279', '\u027A', '\u027B', '\u027C', '\u027D', '\u027E', '\u027F',
        '\u0280', '\u0281', '\u0282', '\u0283', '\u0284', '\u0285', '\u0286', '\u0287', '\u0288', '\u0289', '\u028A', '\u028B', '\u028C', '\u028D', '\u028E', '\u028F',
        '\u0290', '\u0291', '\u0292', '\u0293', '\u0294', '\u0295', '\u0296', '\u0297', '\u0298', '\u0299', '\u029A', '\u029B', '\u029C', '\u029D', '\u029E', '\u029F',
        '\u02A0', '\u02A1', '\u02A2', '\u02A3', '\u02A4', '\u02A5', '\u02A6', '\u02A7', '\u02A8', '\u02A9', '\u02AA', '\u02AB', '\u02AC', '\u02AD', '\u02AE', '\u02AF',
        '\u02B0', '\u02B1', '\u02B2', '\u02B3', '\u02B4', '\u02B5', '\u02B6', '\u02B7', '\u02B8', '\u02B9', '\u02BA', '\u02BB', '\u02BC', '\u02BD', '\u02BE', '\u02BF',
        '\u02C0', '\u02C1', '\u02C2', '\u02C3', '\u02C4', '\u02C5', '\u02C6', '\u02C7', '\u02C8', '\u02C9', '\u02CA', '\u02CB', '\u02CC', '\u02CD', '\u02CE', '\u02CF',
        '\u02D0', '\u02D1', '\u02D2', '\u02D3', '\u02D4', '\u02D5', '\u02D6', '\u02D7', '\u02D8', '\u02D9', '\u02DA', '\u02DB', '\u02DC', '\u02DD', '\u02DE', '\u02DF',
        '\u02E0', '\u02E1', '\u02E2', '\u02E3', '\u02E4', '\u02E5', '\u02E6', '\u02E7', '\u02E8', '\u02E9', '\u02EA', '\u02EB', '\u02EC', '\u02ED', '\u02EE', '\u02EF',
        '\u02F0', '\u02F1', '\u02F2', '\u02F3', '\u02F4', '\u02F5', '\u02F6', '\u02F7', '\u02F8', '\u02F9', '\u02FA', '\u02FB', '\u02FC', '\u02FD', '\u02FE', '\u02FF',
        '\u0300', '\u0301', '\u0302', '\u0303', '\u0304', '\u0305', '\u0306', '\u0307', '\u0308', '\u0309', '\u030A', '\u030B', '\u030C', '\u030D', '\u030E', '\u030F',
        '\u0310', '\u0311', '\u0312', '\u0313', '\u0314', '\u0315', '\u0316', '\u0317', '\u0318', '\u0319', '\u031A', '\u031B', '\u031C', '\u031D', '\u031E', '\u031F',
        '\u0320', '\u0321', '\u0322', '\u0323', '\u0324', '\u0325', '\u0326', '\u0327', '\u0328', '\u0329', '\u032A', '\u032B', '\u032C', '\u032D', '\u032E', '\u032F',
        '\u0330', '\u0331', '\u0332', '\u0333', '\u0334', '\u0335', '\u0336', '\u0337', '\u0338', '\u0339', '\u033A', '\u033B', '\u033C', '\u033D', '\u033E', '\u033F',
        '\u0340', '\u0341', '\u0342', '\u0343', '\u0344', '\u0345', '\u0346', '\u0347', '\u0348', '\u0349', '\u034A', '\u034B', '\u034C', '\u034D', '\u034E', '\u034F',
        '\u0350', '\u0351', '\u0352', '\u0353', '\u0354', '\u0355', '\u0356', '\u0357', '\u0358', '\u0359', '\u035A', '\u035B', '\u035C', '\u035D', '\u035E', '\u035F',
        '\u0360', '\u0361', '\u0362', '\u0363', '\u0364', '\u0365', '\u0366', '\u0367', '\u0368', '\u0369', '\u036A', '\u036B', '\u036C', '\u036D', '\u036E', '\u036F',
        '\u0374', '\u0375', '\u037A', '\u037B', '\u037C', '\u037D', '\u037E',
        '\u0384', '\u0385', '\u0386', '\u0387', '\u0388', '\u0389', '\u038A', '\u038C', '\u038E', '\u038F',
        '\u0390', '\u0391', '\u0392', '\u0393', '\u0394', '\u0395', '\u0396', '\u0397', '\u0398', '\u0399', '\u039A', '\u039B', '\u039C', '\u039D', '\u039E', '\u039F',
        '\u03A0', '\u03A1', '\u03A3', '\u03A4', '\u03A5', '\u03A6', '\u03A7', '\u03A8', '\u03A9', '\u03AA', '\u03AB', '\u03AC', '\u03AD', '\u03AE', '\u03AF',
        '\u03B0', '\u03B1', '\u03B2', '\u03B3', '\u03B4', '\u03B5', '\u03B6', '\u03B7', '\u03B8', '\u03B9', '\u03BA', '\u03BB', '\u03BC', '\u03BD', '\u03BE', '\u03BF',
        '\u03C0', '\u03C1', '\u03C2', '\u03C3', '\u03C4', '\u03C5', '\u03C6', '\u03C7', '\u03C8', '\u03C9', '\u03CA', '\u03CB', '\u03CC', '\u03CD', '\u03CE',
        '\u03D0', '\u03D1', '\u03D2', '\u03D3', '\u03D4', '\u03D5', '\u03D6', '\u03D7', '\u03D8', '\u03D9', '\u03DA', '\u03DB', '\u03DC', '\u03DD', '\u03DE', '\u03DF',
        '\u03E0', '\u03E1', '\u03E2', '\u03E3', '\u03E4', '\u03E5', '\u03E6', '\u03E7', '\u03E8', '\u03E9', '\u03EA', '\u03EB', '\u03EC', '\u03ED', '\u03EE', '\u03EF',
        '\u03F0', '\u03F1', '\u03F2', '\u03F3', '\u03F4', '\u03F5', '\u03F6', '\u03F7', '\u03F8', '\u03F9', '\u03FA', '\u03FB', '\u03FC', '\u03FD', '\u03FE', '\u03FF',
        '\u0400', '\u0401', '\u0402', '\u0403', '\u0404', '\u0405', '\u0406', '\u0407', '\u0408', '\u0409', '\u040A', '\u040B', '\u040C', '\u040D', '\u040E', '\u040F',
        '\u0410', '\u0411', '\u0412', '\u0413', '\u0414', '\u0415', '\u0416', '\u0417', '\u0418', '\u0419', '\u041A', '\u041B', '\u041C', '\u041D', '\u041E', '\u041F',
        '\u0420', '\u0421', '\u0422', '\u0423', '\u0424', '\u0425', '\u0426', '\u0427', '\u0428', '\u0429', '\u042A', '\u042B', '\u042C', '\u042D', '\u042E', '\u042F',
        '\u0430', '\u0431', '\u0432', '\u0433', '\u0434', '\u0435', '\u0436', '\u0437', '\u0438', '\u0439', '\u043A', '\u043B', '\u043C', '\u043D', '\u043E', '\u043F',
        '\u0440', '\u0441', '\u0442', '\u0443', '\u0444', '\u0445', '\u0446', '\u0447', '\u0448', '\u0449', '\u044A', '\u044B', '\u044C', '\u044D', '\u044E', '\u044F',
        '\u0450', '\u0451', '\u0452', '\u0453', '\u0454', '\u0455', '\u0456', '\u0457', '\u0458', '\u0459', '\u045A', '\u045B', '\u045C', '\u045D', '\u045E', '\u045F',
        '\u0460', '\u0461', '\u0462', '\u0463', '\u0464', '\u0465', '\u0466', '\u0467', '\u0468', '\u0469', '\u046A', '\u046B', '\u046C', '\u046D', '\u046E', '\u046F',
        '\u0470', '\u0471', '\u0472', '\u0473', '\u0474', '\u0475', '\u0476', '\u0477', '\u0478', '\u0479', '\u047A', '\u047B', '\u047C', '\u047D', '\u047E', '\u047F',
        '\u0480', '\u0481', '\u0482', '\u0483', '\u0484', '\u0485', '\u0486', '\u0487', '\u0488', '\u0489', '\u048A', '\u048B', '\u048C', '\u048D', '\u048E', '\u048F',
        '\u0490', '\u0491', '\u0492', '\u0493', '\u0494', '\u0495', '\u0496', '\u0497', '\u0498', '\u0499', '\u049A', '\u049B', '\u049C', '\u049D', '\u049E', '\u049F',
        '\u04A0', '\u04A1', '\u04A2', '\u04A3', '\u04A4', '\u04A5', '\u04A6', '\u04A7', '\u04A8', '\u04A9', '\u04AA', '\u04AB', '\u04AC', '\u04AD', '\u04AE', '\u04AF',
        '\u04B0', '\u04B1', '\u04B2', '\u04B3', '\u04B4', '\u04B5', '\u04B6', '\u04B7', '\u04B8', '\u04B9', '\u04BA', '\u04BB', '\u04BC', '\u04BD', '\u04BE', '\u04BF',
        '\u04C0', '\u04C1', '\u04C2', '\u04C3', '\u04C4', '\u04C5', '\u04C6', '\u04C7', '\u04C8', '\u04C9', '\u04CA', '\u04CB', '\u04CC', '\u04CD', '\u04CE', '\u04CF',
        '\u04D0', '\u04D1', '\u04D2', '\u04D3', '\u04D4', '\u04D5', '\u04D6', '\u04D7', '\u04D8', '\u04D9', '\u04DA', '\u04DB', '\u04DC', '\u04DD', '\u04DE', '\u04DF',
        '\u04E0', '\u04E1', '\u04E2', '\u04E3', '\u04E4', '\u04E5', '\u04E6', '\u04E7', '\u04E8', '\u04E9', '\u04EA', '\u04EB', '\u04EC', '\u04ED', '\u04EE', '\u04EF',
        '\u04F0', '\u04F1', '\u04F2', '\u04F3', '\u04F4', '\u04F5', '\u04F6', '\u04F7', '\u04F8', '\u04F9', '\u04FA', '\u04FB', '\u04FC', '\u04FD', '\u04FE', '\u04FF',
        '\u0500', '\u0501', '\u0502', '\u0503', '\u0504', '\u0505', '\u0506', '\u0507', '\u0508', '\u0509', '\u050A', '\u050B', '\u050C', '\u050D', '\u050E', '\u050F',
        '\u0510', '\u0511', '\u0512', '\u0513', '\u051A', '\u051B', '\u051C', '\u051D',
        '\u0591', '\u0592', '\u0593', '\u0594', '\u0595', '\u0596', '\u0597', '\u0598', '\u0599', '\u059A', '\u059B', '\u059C', '\u059D', '\u059E', '\u059F',
        '\u05A0', '\u05A1', '\u05A2', '\u05A3', '\u05A4', '\u05A5', '\u05A6', '\u05A7', '\u05A8', '\u05A9', '\u05AA', '\u05AB', '\u05AC', '\u05AD', '\u05AE', '\u05AF',
        '\u05B0', '\u05B1', '\u05B2', '\u05B3', '\u05B4', '\u05B5', '\u05B6', '\u05B7', '\u05B8', '\u05B9', '\u05BA', '\u05BB', '\u05BC', '\u05BD', '\u05BE', '\u05BF',
        '\u05C0', '\u05C1', '\u05C2', '\u05C3', '\u05C4', '\u05C5', '\u05C6', '\u05C7',
        '\u05D0', '\u05D1', '\u05D2', '\u05D3', '\u05D4', '\u05D5', '\u05D6', '\u05D7', '\u05D8', '\u05D9', '\u05DA', '\u05DB', '\u05DC', '\u05DD', '\u05DE', '\u05DF',
        '\u05E0', '\u05E1', '\u05E2', '\u05E3', '\u05E4', '\u05E5', '\u05E6', '\u05E7', '\u05E8', '\u05E9', '\u05EA',
        '\u05F0', '\u05F1', '\u05F2', '\u05F3', '\u05F4',
        '\u1D00', '\u1D01', '\u1D02', '\u1D03', '\u1D04', '\u1D05', '\u1D06', '\u1D07', '\u1D08', '\u1D09', '\u1D0A', '\u1D0B', '\u1D0C', '\u1D0D', '\u1D0E', '\u1D0F',
        '\u1D10', '\u1D11', '\u1D12', '\u1D13', '\u1D14', '\u1D15', '\u1D16', '\u1D17', '\u1D18', '\u1D19', '\u1D1A', '\u1D1B', '\u1D1C', '\u1D1D', '\u1D1E', '\u1D1F',
        '\u1D20', '\u1D21', '\u1D22', '\u1D23', '\u1D24', '\u1D25', '\u1D26', '\u1D27', '\u1D28', '\u1D29', '\u1D2A', '\u1D2B', '\u1D2C', '\u1D2D', '\u1D2E', '\u1D2F',
        '\u1D30', '\u1D31', '\u1D32', '\u1D33', '\u1D34', '\u1D35', '\u1D36', '\u1D37', '\u1D38', '\u1D39', '\u1D3A', '\u1D3B', '\u1D3C', '\u1D3D', '\u1D3E', '\u1D3F',
        '\u1D40', '\u1D41', '\u1D42', '\u1D43', '\u1D44', '\u1D45', '\u1D46', '\u1D47', '\u1D48', '\u1D49', '\u1D4A', '\u1D4B', '\u1D4C', '\u1D4D', '\u1D4E', '\u1D4F',
        '\u1D50', '\u1D51', '\u1D52', '\u1D53', '\u1D54', '\u1D55', '\u1D56', '\u1D57', '\u1D58', '\u1D59', '\u1D5A', '\u1D5B', '\u1D5C', '\u1D5D', '\u1D5E', '\u1D5F',
        '\u1D60', '\u1D61', '\u1D62', '\u1D63', '\u1D64', '\u1D65', '\u1D66', '\u1D67', '\u1D68', '\u1D69', '\u1D6A', '\u1D6B', '\u1D6C', '\u1D6D', '\u1D6E', '\u1D6F',
        '\u1D70', '\u1D71', '\u1D72', '\u1D73', '\u1D74', '\u1D75', '\u1D76', '\u1D77', '\u1D78', '\u1D79', '\u1D7A', '\u1D7B', '\u1D7C', '\u1D7D', '\u1D7E', '\u1D7F',
        '\u1D80', '\u1D81', '\u1D82', '\u1D83', '\u1D84', '\u1D85', '\u1D86', '\u1D87', '\u1D88', '\u1D89', '\u1D8A', '\u1D8B', '\u1D8C', '\u1D8D', '\u1D8E', '\u1D8F',
        '\u1D90', '\u1D91', '\u1D92', '\u1D93', '\u1D94', '\u1D95', '\u1D96', '\u1D97', '\u1D98', '\u1D99', '\u1D9A', '\u1D9B', '\u1D9C', '\u1D9D', '\u1D9E', '\u1D9F',
        '\u1DA0', '\u1DA1', '\u1DA2', '\u1DA3', '\u1DA4', '\u1DA5', '\u1DA6', '\u1DA7', '\u1DA8', '\u1DA9', '\u1DAA', '\u1DAB', '\u1DAC', '\u1DAD', '\u1DAE', '\u1DAF',
        '\u1DB0', '\u1DB1', '\u1DB2', '\u1DB3', '\u1DB4', '\u1DB5', '\u1DB6', '\u1DB7', '\u1DB8', '\u1DB9', '\u1DBA', '\u1DBB', '\u1DBC', '\u1DBD', '\u1DBE', '\u1DBF',
        '\u1DC0', '\u1DC1', '\u1DC2', '\u1DC3', '\u1DC4', '\u1DC5', '\u1DC6', '\u1DC7', '\u1DC8', '\u1DC9', '\u1DCA',
        '\u1DFE', '\u1DFF',
        '\u1E00', '\u1E01', '\u1E02', '\u1E03', '\u1E04', '\u1E05', '\u1E06', '\u1E07', '\u1E08', '\u1E09', '\u1E0A', '\u1E0B', '\u1E0C', '\u1E0D', '\u1E0E', '\u1E0F',
        '\u1E10', '\u1E11', '\u1E12', '\u1E13', '\u1E14', '\u1E15', '\u1E16', '\u1E17', '\u1E18', '\u1E19', '\u1E1A', '\u1E1B', '\u1E1C', '\u1E1D', '\u1E1E', '\u1E1F',
        '\u1E20', '\u1E21', '\u1E22', '\u1E23', '\u1E24', '\u1E25', '\u1E26', '\u1E27', '\u1E28', '\u1E29', '\u1E2A', '\u1E2B', '\u1E2C', '\u1E2D', '\u1E2E', '\u1E2F',
        '\u1E30', '\u1E31', '\u1E32', '\u1E33', '\u1E34', '\u1E35', '\u1E36', '\u1E37', '\u1E38', '\u1E39', '\u1E3A', '\u1E3B', '\u1E3C', '\u1E3D', '\u1E3E', '\u1E3F',
        '\u1E40', '\u1E41', '\u1E42', '\u1E43', '\u1E44', '\u1E45', '\u1E46', '\u1E47', '\u1E48', '\u1E49', '\u1E4A', '\u1E4B', '\u1E4C', '\u1E4D', '\u1E4E', '\u1E4F',
        '\u1E50', '\u1E51', '\u1E52', '\u1E53', '\u1E54', '\u1E55', '\u1E56', '\u1E57', '\u1E58', '\u1E59', '\u1E5A', '\u1E5B', '\u1E5C', '\u1E5D', '\u1E5E', '\u1E5F',
        '\u1E60', '\u1E61', '\u1E62', '\u1E63', '\u1E64', '\u1E65', '\u1E66', '\u1E67', '\u1E68', '\u1E69', '\u1E6A', '\u1E6B', '\u1E6C', '\u1E6D', '\u1E6E', '\u1E6F',
        '\u1E70', '\u1E71', '\u1E72', '\u1E73', '\u1E74', '\u1E75', '\u1E76', '\u1E77', '\u1E78', '\u1E79', '\u1E7A', '\u1E7B', '\u1E7C', '\u1E7D', '\u1E7E', '\u1E7F',
        '\u1E80', '\u1E81', '\u1E82', '\u1E83', '\u1E84', '\u1E85', '\u1E86', '\u1E87', '\u1E88', '\u1E89', '\u1E8A', '\u1E8B', '\u1E8C', '\u1E8D', '\u1E8E', '\u1E8F',
        '\u1E90', '\u1E91', '\u1E92', '\u1E93', '\u1E94', '\u1E95', '\u1E96', '\u1E97', '\u1E98', '\u1E99', '\u1E9A', '\u1E9B', '\u1E9E',
        '\u1EA0', '\u1EA1', '\u1EA2', '\u1EA3', '\u1EA4', '\u1EA5', '\u1EA6', '\u1EA7', '\u1EA8', '\u1EA9', '\u1EAA', '\u1EAB', '\u1EAC', '\u1EAD', '\u1EAE', '\u1EAF',
        '\u1EB0', '\u1EB1', '\u1EB2', '\u1EB3', '\u1EB4', '\u1EB5', '\u1EB6', '\u1EB7', '\u1EB8', '\u1EB9', '\u1EBA', '\u1EBB', '\u1EBC', '\u1EBD', '\u1EBE', '\u1EBF',
        '\u1EC0', '\u1EC1', '\u1EC2', '\u1EC3', '\u1EC4', '\u1EC5', '\u1EC6', '\u1EC7', '\u1EC8', '\u1EC9', '\u1ECA', '\u1ECB', '\u1ECC', '\u1ECD', '\u1ECE', '\u1ECF',
        '\u1ED0', '\u1ED1', '\u1ED2', '\u1ED3', '\u1ED4', '\u1ED5', '\u1ED6', '\u1ED7', '\u1ED8', '\u1ED9', '\u1EDA', '\u1EDB', '\u1EDC', '\u1EDD', '\u1EDE', '\u1EDF',
        '\u1EE0', '\u1EE1', '\u1EE2', '\u1EE3', '\u1EE4', '\u1EE5', '\u1EE6', '\u1EE7', '\u1EE8', '\u1EE9', '\u1EEA', '\u1EEB', '\u1EEC', '\u1EED', '\u1EEE', '\u1EEF',
        '\u1EF0', '\u1EF1', '\u1EF2', '\u1EF3', '\u1EF4', '\u1EF5', '\u1EF6', '\u1EF7', '\u1EF8', '\u1EF9',
        '\u1F00', '\u1F01', '\u1F02', '\u1F03', '\u1F04', '\u1F05', '\u1F06', '\u1F07', '\u1F08', '\u1F09', '\u1F0A', '\u1F0B', '\u1F0C', '\u1F0D', '\u1F0E', '\u1F0F',
        '\u1F10', '\u1F11', '\u1F12', '\u1F13', '\u1F14', '\u1F15', '\u1F18', '\u1F19', '\u1F1A', '\u1F1B', '\u1F1C', '\u1F1D',
        '\u1F20', '\u1F21', '\u1F22', '\u1F23', '\u1F24', '\u1F25', '\u1F26', '\u1F27', '\u1F28', '\u1F29', '\u1F2A', '\u1F2B', '\u1F2C', '\u1F2D', '\u1F2E', '\u1F2F',
        '\u1F30', '\u1F31', '\u1F32', '\u1F33', '\u1F34', '\u1F35', '\u1F36', '\u1F37', '\u1F38', '\u1F39', '\u1F3A', '\u1F3B', '\u1F3C', '\u1F3D', '\u1F3E', '\u1F3F',
        '\u1F40', '\u1F41', '\u1F42', '\u1F43', '\u1F44', '\u1F45', '\u1F48', '\u1F49', '\u1F4A', '\u1F4B', '\u1F4C', '\u1F4D',
        '\u1F50', '\u1F51', '\u1F52', '\u1F53', '\u1F54', '\u1F55', '\u1F56', '\u1F57', '\u1F59', '\u1F5B', '\u1F5D', '\u1F5F',
        '\u1F60', '\u1F61', '\u1F62', '\u1F63', '\u1F64', '\u1F65', '\u1F66', '\u1F67', '\u1F68', '\u1F69', '\u1F6A', '\u1F6B', '\u1F6C', '\u1F6D', '\u1F6E', '\u1F6F',
        '\u1F70', '\u1F71', '\u1F72', '\u1F73', '\u1F74', '\u1F75', '\u1F76', '\u1F77', '\u1F78', '\u1F79', '\u1F7A', '\u1F7B', '\u1F7C', '\u1F7D',
        '\u1F80', '\u1F81', '\u1F82', '\u1F83', '\u1F84', '\u1F85', '\u1F86', '\u1F87', '\u1F88', '\u1F89', '\u1F8A', '\u1F8B', '\u1F8C', '\u1F8D', '\u1F8E', '\u1F8F',
        '\u1F90', '\u1F91', '\u1F92', '\u1F93', '\u1F94', '\u1F95', '\u1F96', '\u1F97', '\u1F98', '\u1F99', '\u1F9A', '\u1F9B', '\u1F9C', '\u1F9D', '\u1F9E', '\u1F9F',
        '\u1FA0', '\u1FA1', '\u1FA2', '\u1FA3', '\u1FA4', '\u1FA5', '\u1FA6', '\u1FA7', '\u1FA8', '\u1FA9', '\u1FAA', '\u1FAB', '\u1FAC', '\u1FAD', '\u1FAE', '\u1FAF',
        '\u1FB0', '\u1FB1', '\u1FB2', '\u1FB3', '\u1FB4', '\u1FB6', '\u1FB7', '\u1FB8', '\u1FB9', '\u1FBA', '\u1FBB', '\u1FBC', '\u1FBD', '\u1FBE', '\u1FBF',
        '\u1FC0', '\u1FC1', '\u1FC2', '\u1FC3', '\u1FC4', '\u1FC6', '\u1FC7', '\u1FC8', '\u1FC9', '\u1FCA', '\u1FCB', '\u1FCC', '\u1FCD', '\u1FCE', '\u1FCF',
        '\u1FD0', '\u1FD1', '\u1FD2', '\u1FD3', '\u1FD6', '\u1FD7', '\u1FD8', '\u1FD9', '\u1FDA', '\u1FDB', '\u1FDD', '\u1FDE', '\u1FDF',
        '\u1FE0', '\u1FE1', '\u1FE2', '\u1FE3', '\u1FE4', '\u1FE5', '\u1FE6', '\u1FE7', '\u1FE8', '\u1FE9', '\u1FEA', '\u1FEB', '\u1FEC', '\u1FED', '\u1FEE', '\u1FEF',
        '\u1FF2', '\u1FF3', '\u1FF4', '\u1FF6', '\u1FF7', '\u1FF8', '\u1FF9', '\u1FFA', '\u1FFB', '\u1FFC', '\u1FFD', '\u1FFE',
        '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A', '\u200B', '\u200C', '\u200D', '\u200E', '\u200F',
        '\u2012', '\u2013', '\u2014', '\u2015', '\u2016', '\u2017', '\u2018', '\u2019', '\u201A', '\u201B', '\u201C', '\u201D', '\u201E', '\u201F',
        '\u2020', '\u2021', '\u2022', '\u2026', '\u202A', '\u202B', '\u202C', '\u202D', '\u202E', '\u202F',
        '\u2030', '\u2032', '\u2033', '\u2034', '\u2039', '\u203A', '\u203C', '\u203E',
        '\u2044', '\u205E', '\u206A', '\u206B', '\u206C', '\u206D', '\u206E', '\u206F',
        '\u2074', '\u2075', '\u2077', '\u2078', '\u207F',
        '\u2090', '\u2091', '\u2092', '\u2093', '\u2094',
        '\u20A0', '\u20A1', '\u20A2', '\u20A3', '\u20A4', '\u20A5', '\u20A6', '\u20A7', '\u20A8', '\u20A9', '\u20AA', '\u20AB', '\u20AC', '\u20AD', '\u20AE', '\u20AF',
        '\u20B0', '\u20B1', '\u20B2', '\u20B3', '\u20B4', '\u20B5',
        '\u20F0', '\u2105', '\u2113', '\u2116', '\u2117',
        '\u2122', '\u2126', '\u212E',
        '\u214D', '\u214E',
        '\u2153', '\u2154', '\u215B', '\u215C', '\u215D', '\u215E',
        '\u2184', '\u2190', '\u2191', '\u2192', '\u2193', '\u2194', '\u2195',
        '\u21A8', '\u2202', '\u2206', '\u220F',
        '\u2211', '\u2212', '\u2215', '\u2219', '\u221A', '\u221E', '\u221F',
        '\u2229', '\u222B',
        '\u2248', '\u2260', '\u2261', '\u2264', '\u2265',
        '\u2302', '\u2310', '\u2320', '\u2321',
        '\u2500', '\u2502', '\u250C',
        '\u2510', '\u2514', '\u2518', '\u251C',
        '\u2524', '\u252C',
        '\u2534', '\u253C',
        '\u2550', '\u2551', '\u2552', '\u2553', '\u2554', '\u2555', '\u2556', '\u2557', '\u2558', '\u2559', '\u255A', '\u255B', '\u255C', '\u255D', '\u255E', '\u255F',
        '\u2560', '\u2561', '\u2562', '\u2563', '\u2564', '\u2565', '\u2566', '\u2567', '\u2568', '\u2569', '\u256A', '\u256B', '\u256C',
        '\u2580', '\u2584', '\u2588', '\u258C',
        '\u2590', '\u2591', '\u2592', '\u2593',
        '\u25A0', '\u25A1', '\u25AA', '\u25AB', '\u25AC',
        '\u25B2', '\u25BA', '\u25BC',
        '\u25C4', '\u25CA', '\u25CB', '\u25CC', '\u25CF',
        '\u25D8', '\u25D9',
        '\u25E6', '\u263A', '\u263B', '\u263C',
        '\u2640', '\u2642',
        '\u2660', '\u2663', '\u2665', '\u2666', '\u266A', '\u266B', '\u266F',
        '\u2C60', '\u2C61', '\u2C62', '\u2C63', '\u2C64', '\u2C65', '\u2C66', '\u2C67', '\u2C68', '\u2C69', '\u2C6A', '\u2C6B', '\u2C6C', '\u2C6D',
        '\u2C71', '\u2C72', '\u2C73', '\u2C74', '\u2C75', '\u2C76', '\u2C77',
        '\u2E17', '\uA717', '\uA718', '\uA719', '\uA71A', '\uA71B', '\uA71C', '\uA71D', '\uA71E', '\uA71F',
        '\uA720', '\uA721',
        '\uA788', '\uA789', '\uA78A', '\uA78B', '\uA78C',
        '\uFB01', '\uFB02',
        '\uFB1D', '\uFB1E', '\uFB1F',
        '\uFB20', '\uFB21', '\uFB22', '\uFB23', '\uFB24', '\uFB25', '\uFB26', '\uFB27', '\uFB28', '\uFB29', '\uFB2A', '\uFB2B', '\uFB2C', '\uFB2D', '\uFB2E', '\uFB2F',
        '\uFB30', '\uFB31', '\uFB32', '\uFB33', '\uFB34', '\uFB35', '\uFB36', '\uFB38', '\uFB39', '\uFB3A', '\uFB3B', '\uFB3C', '\uFB3E',
        '\uFB40', '\uFB41', '\uFB43', '\uFB44', '\uFB46', '\uFB47', '\uFB48', '\uFB49', '\uFB4A', '\uFB4B', '\uFB4C', '\uFB4D', '\uFB4E', '\uFB4F',
        '\uFE20', '\uFE21', '\uFE22', '\uFE23',
        '\uFFFC'
    ];

    public static ReadOnlySpan<char> DefinedCHSExt => [
        '\u0020', '\u002D', '\u003F', '\u0067', '\u00AA', '\u00B7', '\u00E9',
        '\u2013', '\u2018', '\u2019', '\u201C', '\u201D', '\u201E', '\u2026', '\u20BD', '\u21D2', '\u21D4', '\u2200', '\u2282', '\u2283',
        '\u25A0', '\u25BC', '\u25BD', '\u25CF', '\u2605', '\u2661', '\u2665', '\u266A', '\u266D',
        '\u300A', '\u300B', '\u300C', '\u300D', '\u300E', '\u300F', '\u3010', '\u3011', '\u30FB', '\uFF08', '\uFF09', '\uFF65'
    ];
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using HW.Account.DAL.Data;
using HW.Account.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HW.Account.BLL.Services;

/// <summary>
/// Service for authentication and authorization
/// </summary>
public class AuthService : IAuthService {
    private readonly ILogger<AuthService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly AccountDbContext _accountDbContext;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="accountDbContext"></param>
    /// <param name="configuration"></param>
    /// <param name="emailService"></param>
    public AuthService(ILogger<AuthService> logger, UserManager<User> userManager, SignInManager<User> signInManager,
        AccountDbContext accountDbContext,IConfiguration configuration, IEmailService emailService) {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _accountDbContext = accountDbContext;
        _configuration = configuration;
        _emailService = emailService;
        
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="accountRegisterDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> RegisterAsync(AccountRegisterDto accountRegisterDto, HttpContext httpContext) {
        if (accountRegisterDto.Email == null) {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Email is empty");
        }

        if (accountRegisterDto.Password == null) {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Password is empty");
        }

        if (await _userManager.FindByEmailAsync(accountRegisterDto.Email) != null) {
            throw new ConflictException("User with this email already exists");
        }

        var user = new User {
            Email = accountRegisterDto.Email,
            NickName = accountRegisterDto.NickName,
            UserName = accountRegisterDto.Email,
            FullName = accountRegisterDto.FullName,
        };
        user.BirthDate = new BirthDate {
            Value = accountRegisterDto.BirthDate,
            User = user
        };
        user.Location = new Location {
            User = user
        };
        user.Education = new Education {
            User = user
        };
        user.WorkExperience = new WorkExperience {
            User = user
        };
        var result = await _userManager.CreateAsync(user, accountRegisterDto.Password);

        if (result.Succeeded) {
           var newUser = await _userManager.FindByIdAsync(user.Id.ToString());
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            
            var encode = HttpUtility.UrlEncode(code);
            var config = _configuration.GetSection("ConfirmMVCUrl");
            try {
                await _emailService.SendEmailAsync(user.Email, "Подтверждение почты",
                    $"\r\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"ru\"><head><meta charset=\"UTF-8\"><meta content=\"width=device-width, initial-scale=1\" name=\"viewport\"><meta name=\"x-apple-disable-message-reformatting\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta content=\"telephone=no\" name=\"format-detection\"><title>Новый шаблон</title> <!--[if (mso 16)]><style type=\"text/css\">     a {{text-decoration: none;}}     </style><![endif]--> <!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]--> <!--[if gte mso 9]><xml> <o:OfficeDocumentSettings> <o:AllowPNG></o:AllowPNG> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml>\r\n<![endif]--><style type=\"text/css\">.rollover:hover .rollover-first {{ max-height:0px!important; display:none!important; }} .rollover:hover .rollover-second {{ max-height:none!important; display:inline-block!important; }} .rollover div {{ font-size:0px; }} u + .body img ~ div div {{ display:none; }} #outlook a {{ padding:0; }} span.MsoHyperlink,span.MsoHyperlinkFollowed {{ color:inherit; mso-style-priority:99; }} a.es-button {{ mso-style-priority:100!important; text-decoration:none!important; }} a[x-apple-data-detectors] {{ color:inherit!important; text-decoration:none!important; font-size:inherit!important; font-family:inherit!important; font-weight:inherit!important; line-height:inherit!important; }} .es-desk-hidden {{ display:none; float:left; overflow:hidden; width:0; max-height:0; line-height:0; mso-hide:all; }} .es-button-border:hover > a.es-button {{ color:#ffffff!important; }}\r\n@media only screen and (max-width:600px) {{.es-m-p0r {{ padding-right:0px!important }} .es-m-p0l {{ padding-left:0px!important }} .es-m-p0r {{ padding-right:0px!important }} .es-m-p0l {{ padding-left:0px!important }} *[class=\"gmail-fix\"] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} h1 {{ font-size:36px!important; text-align:left }} h2 {{ font-size:26px!important; text-align:left }} h3 {{ font-size:20px!important; text-align:left }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:36px!important }}\r\n .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:26px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:12px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:16px!important }} .es-footer-body p, .es-footer-body a {{ font-size:14px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }}\r\n .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover div, .es-m-txt-c .rollover div, .es-m-txt-l .rollover div {{ line-height:0!important; font-size:0!important }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:20px!important; line-height:120%!important }}\r\n a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }}\r\n table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .es-social td {{ padding-bottom:10px }} .h-auto {{ height:auto!important }} }}</style>\r\n </head> <body class=\"body\" style=\"width:100%;height:100%;padding:0;Margin:0\"><div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"ru\" style=\"background-color:#FAFAFA\"> <!--[if gte mso 9]><v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\"> <v:fill type=\"tile\" color=\"#fafafa\"></v:fill> </v:background><![endif]--><table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FAFAFA\"><tr><td valign=\"top\" style=\"padding:0;Margin:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\"><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\"><tr><td align=\"left\" style=\"Margin:0;padding-top:30px;padding-right:20px;padding-bottom:30px;padding-left:20px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px;font-size:0px\"><a target=\"_blank\" href=\"https://hw.hw-online.front.freydin.space/products\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img src=\"https://hwtpu.ru/wp-content/uploads/2022/12/hwtpu_logo_ru-2022-3.png\" alt=\"\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\" width=\"370\"></a> </td></tr><tr><td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><h1 style=\"Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:46px;font-style:normal;font-weight:bold;line-height:69px !important;color:#333333\">Подтверждение почты</h1></td></tr> <tr>\r\n<td align=\"center\" class=\"es-m-p0r es-m-p0l\" style=\"Margin:0;padding-top:5px;padding-right:40px;padding-bottom:5px;padding-left:40px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">Вы получили это сообщение, потому что ваш адрес электронной почты был зарегистрирован на нашем <a style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:13px;line-height:20px\" target=\"_blank\" href=\"https://hw.hw-online.front.freydin.space/products\">сайте</a>. Нажмите кнопку ниже, чтобы подтвердить свой адрес электронной почты и, что Вы являетесь владельцем этой учетной записи.</p></td></tr> <tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:5px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">Если Вы не регистрировались у нас, пожалуйста, игнорируйте это письмо.</p></td></tr> <tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><span class=\"es-button-border\" style=\"border-style:solid;border-color:#2CB543;background:#349d1f;border-width:0px;display:inline-block;border-radius:6px;width:auto\"><a href=\"{config.GetValue<string>("Url")}?userId={user.Id}&code={encode}\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;color:#FFFFFF;font-size:20px;padding:10px 30px 10px 30px;display:inline-block;background:#349d1f;border-radius:6px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:24px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #349d1f;border-left-width:30px;border-right-width:30px\">ПОДТВЕРЖДЕНИЕ</a></span></td></tr> <tr>\r\n<td align=\"center\" class=\"es-m-p0r es-m-p0l\" style=\"Margin:0;padding-top:5px;padding-right:40px;padding-bottom:5px;padding-left:40px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px !important;letter-spacing:0;color:#333333;font-size:14px\">После подтверждения этот адрес электронной почты будет однозначно связан с вашей учетной записью.</p></td></tr> <tr><td align=\"center\" style=\"padding:0;Margin:0;font-size:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:10px\"><a target=\"_blank\" href=\"https://vk.com/hwtpu\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img title=\"Собственная иконка\" src=\"https://fcuvome.stripocdn.email/content/guids/CABINET_00b1de74d065a08edd1917864384386942a586c96ca49f166e096bb8b6c24ca3/images/_u2egx24fgm.jpg\" alt=\"Собственная иконка\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td>\r\n <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:10px\"><a target=\"_blank\" href=\"https://t.me/hwtpu\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img title=\"Telegram\" src=\"https://fcuvome.stripocdn.email/content/assets/img/messenger-icons/logo-black/telegram-logo-black.png\" alt=\"Telegram\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0\"><a target=\"_blank\" href=\"https://www.youtube.com/@hwtpu_channel\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img title=\"Youtube\" src=\"https://fcuvome.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png\" alt=\"Yt\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td></tr></table></td></tr> </table></td>\r\n</tr></table></td></tr></table></td></tr></table></td></tr></table></div></body></html>");
            }
            catch (Exception e) {
                await _userManager.DeleteAsync(newUser);
                throw new BadRequestException(e.Message);
            }

            await _userManager.AddToRoleAsync(newUser, ApplicationRoleNames.Student);
            _logger.LogInformation("Successful register");
            return await LoginAsync(new AccountLoginDto()
                { Email = accountRegisterDto.Email, Password = accountRegisterDto.Password }, httpContext);
        }

        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        throw new BadRequestException(errors);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="accountLoginDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> LoginAsync(AccountLoginDto accountLoginDto, HttpContext httpContext) {
        var identity = await GetIdentity(accountLoginDto.Email.ToLower(), accountLoginDto.Password);
        if (identity == null) {
            throw new BadRequestException("Incorrect username or password");
        }

        var user = _userManager.Users.Include(x => x.Devices).FirstOrDefault(x => x.Email == accountLoginDto.Email);
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        
        if (await _userManager.IsLockedOutAsync(user)) {
            throw new UnauthorizedException("User is banned");
        }
        
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        
        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress && x.UserAgent == userAgent);

        if (device == null) {
            device = new Device() {
                User = user,
                RefreshToken = $"{Guid.NewGuid()}-{Guid.NewGuid()}",
                UserAgent = userAgent,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };
            await _accountDbContext.Devices.AddAsync(device);
        }

        device.LastActivity = DateTime.UtcNow;
        device.ExpirationDate = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt")
            .GetValue<int>("RefreshTokenLifetimeInDays"));
        
        await _accountDbContext.SaveChangesAsync();

        var jwt = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt")["Issuer"],
            audience: _configuration.GetSection("Jwt")["Audience"],
            notBefore: DateTime.UtcNow,
            claims: identity.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                .GetValue<int>("AccessTokenLifetimeInMinutes"))),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256));

        _logger.LogInformation("Successful login");

        return new TokenResponseDto() {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            RefreshToken = device.RefreshToken
        };
    }

    /// <summary>
    /// Logout user by deleting his current device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task LogoutAsync(Guid userId, HttpContext httpContext) {
        var user = _userManager.Users
            .Include(x => x.Devices)
            .FirstOrDefault(x => x.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        
        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress && x.UserAgent == userAgent);

        if (device == null) {
            throw new MethodNotAllowedException("You can`t logout from this device");
        }

        _accountDbContext.Devices.Remove(device);
        await _accountDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="tokenRequestDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto, HttpContext httpContext) {
        tokenRequestDto.AccessToken = tokenRequestDto.AccessToken.Replace("Bearer ", "");
        var principal = GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
        if (principal.Identity == null) {
            throw new BadRequestException("Invalid jwt token");
        }

        var user = _userManager.Users.Include(x => x.Devices)
            .FirstOrDefault(x => x.Id.ToString() == principal.Identity.Name);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        if (await _userManager.IsLockedOutAsync(user)) {
            throw new UnauthorizedException("User is banned");
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        
        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress && x.UserAgent == userAgent);

        if (device == null) {
            throw new MethodNotAllowedException("You can't refresh token from another device. Re-login needed");
        }

        if (device.RefreshToken != tokenRequestDto.RefreshToken) {
            throw new BadRequestException("Refresh token is invalid");
        }

        if (device.ExpirationDate < DateTime.UtcNow) {
            throw new UnauthorizedException("Refresh token is expired. Re-login needed");
        }

        var jwt = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt")["Issuer"],
            audience: null,
            notBefore: DateTime.UtcNow,
            claims: principal.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                .GetValue<int>("AccessTokenLifetimeInMinutes"))),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256));

        device.LastActivity = DateTime.UtcNow;
        device.ExpirationDate = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt")
            .GetValue<int>("RefreshTokenLifetimeInDays"));
        await _accountDbContext.SaveChangesAsync();

        return new TokenResponseDto() {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            RefreshToken = device.RefreshToken
        };
    }

    /// <summary>
    /// Get user devices
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<DeviceDto>> GetDevicesAsync(Guid userId) {
        var user = _userManager.Users.Include(x => x.Devices).FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        return Task.FromResult(user.Devices.Select(d => new DeviceDto {
            DeviceName = d.DeviceName,
            IpAddress = d.IpAddress,
            UserAgent = d.UserAgent,
            LastActivity = d.LastActivity,
            Id = d.Id,
        }).ToList());
    }

    /// <summary>
    /// Rename device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="deviceRenameDto"></param>
    /// <returns></returns>
    public async Task RenameDeviceAsync(Guid userId, Guid deviceId, DeviceRenameDto deviceRenameDto) {
        var user = _userManager.Users
            .Include(x => x.Devices)
            .FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var device = user.Devices.FirstOrDefault(d => d.Id == deviceId);
        if (device == null) {
            throw new NotFoundException("Device not found");
        }

        device.DeviceName = deviceRenameDto.DeviceName;
        await _accountDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Delete device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    public async Task DeleteDeviceAsync(Guid userId, Guid deviceId) {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var device = _accountDbContext.Devices.FirstOrDefault(d => d.User == user);
        if (device == null) {
            throw new NotFoundException("Device not found");
        }

        _accountDbContext.Devices.Remove(device);
        await _accountDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="changePasswordDto"></param>
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var result =
            await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    /// <summary>
    /// Restore password
    /// </summary>
    /// <param name="emailDto"></param>
    public async Task RestorePasswordAsync(EmailDto emailDto)
    {
        var userM = await _userManager.FindByEmailAsync(emailDto.Email);
        if (userM != null)
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userM);
            var encode = HttpUtility.UrlEncode(resetToken);
            var config = _configuration.GetSection("ResetPasswordMVCUrl");
            try
            {
                await _emailService.SendEmailAsync(emailDto.Email, "Восстановление пароля",
                    $"<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"ru\"><head><meta charset=\"UTF-8\"><meta content=\"width=device-width, initial-scale=1\" name=\"viewport\"><meta name=\"x-apple-disable-message-reformatting\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta content=\"telephone=no\" name=\"format-detection\"><title>Новый шаблон</title> <!--[if (mso 16)]><style type=\"text/css\">     a {{text-decoration: none;}}     </style><![endif]--> <!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]--> <!--[if gte mso 9]><xml><o:OfficeDocumentSettings><o:AllowPNG></o:AllowPNG><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml>\r\n<![endif]--><style type=\"text/css\">.rollover:hover .rollover-first {{ max-height:0px!important; display:none!important; }} .rollover:hover .rollover-second {{ max-height:none!important; display:inline-block!important; }} .rollover div {{ font-size:0px; }} u + .body img ~ div div {{ display:none; }} #outlook a {{ padding:0; }} span.MsoHyperlink,span.MsoHyperlinkFollowed {{ color:inherit; mso-style-priority:99; }} a.es-button {{ mso-style-priority:100!important; text-decoration:none!important; }} a[x-apple-data-detectors] {{ color:inherit!important; text-decoration:none!important; font-size:inherit!important; font-family:inherit!important; font-weight:inherit!important; line-height:inherit!important; }} .es-desk-hidden {{ display:none; float:left; overflow:hidden; width:0; max-height:0; line-height:0; mso-hide:all; }} .es-button-border:hover > a.es-button {{ color:#ffffff!important; }}\r\n@media only screen and (max-width:600px) {{.es-m-p0r {{ padding-right:0px!important }} .es-m-p0l {{ padding-left:0px!important }} .es-m-p0r {{ padding-right:0px!important }} .es-m-p0l {{ padding-left:0px!important }} *[class=\"gmail-fix\"] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} h1 {{ font-size:36px!important; text-align:left }} h2 {{ font-size:26px!important; text-align:left }} h3 {{ font-size:20px!important; text-align:left }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:36px!important }}\r\n .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:26px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:12px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:16px!important }} .es-footer-body p, .es-footer-body a {{ font-size:14px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }}\r\n .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover div, .es-m-txt-c .rollover div, .es-m-txt-l .rollover div {{ line-height:0!important; font-size:0!important }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:20px!important; line-height:120%!important }}\r\n a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }}\r\n table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .es-social td {{ padding-bottom:10px }} .h-auto {{ height:auto!important }} }}.rollover:hover .rollover-first {{ max-height:0px!important; display:none!important; }} .rollover:hover .rollover-second {{ max-height:none!important; display:block!important; }} .rollover span {{ font-size:0px; }} u + .body img ~ div div {{ display:none; }} #outlook a {{ padding:0; }} span.MsoHyperlink,span.MsoHyperlinkFollowed {{ color:inherit; mso-style-priority:99; }} a.es-button {{ mso-style-priority:100!important; text-decoration:none!important; }} a[x-apple-data-detectors] {{ color:inherit!important; text-decoration:none!important; font-size:inherit!important; font-family:inherit!important; font-weight:inherit!important; line-height:inherit!important; }} .es-desk-hidden {{ display:none; float:left; overflow:hidden; width:0; max-height:0; line-height:0; mso-hide:all; }}\r\n .es-button-border:hover > a.es-button {{ color:#ffffff!important; }}@media only screen and (max-width:600px) {{.es-m-p0r {{ padding-right:0px!important }} .es-m-p0l {{ padding-left:0px!important }} .es-m-p0r {{ padding-right:0px!important }} .es-m-p0l {{ padding-left:0px!important }} *[class=\"gmail-fix\"] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} h1 {{ font-size:30px!important; text-align:left }} h2 {{ font-size:24px!important; text-align:left }} h3 {{ font-size:20px!important; text-align:left }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }}\r\n .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:30px!important }} .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:24px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:14px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:14px!important }} .es-footer-body p, .es-footer-body a {{ font-size:14px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }}\r\n .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }} .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover span, .es-m-txt-c .rollover span, .es-m-txt-l .rollover span {{ line-height:0!important; font-size:0!important }}\r\n .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:18px!important; line-height:120%!important }} a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }}\r\n table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }} table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .es-social td {{ padding-bottom:10px }} .h-auto {{ height:auto!important }} }}@media screen and (max-width:384px) {{.mail-message-content {{ width:414px!important }} }}</style>\r\n</head> <body class=\"body\" style=\"width:100%;height:100%;padding:0;Margin:0\"><div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"ru\" style=\"background-color:#FAFAFA\"> <!--[if gte mso 9]><v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\"><v:fill type=\"tile\" color=\"#fafafa\"></v:fill></v:background><![endif]--><table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FAFAFA\"><tr><td valign=\"top\" style=\"padding:0;Margin:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\"><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\"><tr><td align=\"left\" style=\"Margin:0;padding-top:30px;padding-right:20px;padding-bottom:30px;padding-left:20px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px;font-size:0px\"><a target=\"_blank\" href=\"https://hw.hw-online.front.freydin.space/products\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img src=\"https://hwtpu.ru/wp-content/uploads/2022/12/hwtpu_logo_ru-2022-3.png\" alt=\"\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\" width=\"370\"></a> </td></tr><tr><td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><h1 style=\"Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:28px;font-style:normal;font-weight:bold;line-height:69px !important;color:#333333\">Восстановление пароля</h1></td></tr><tr>\r\n<td align=\"center\" class=\"es-m-p0r es-m-p0l\" style=\"Margin:0;padding-top:5px;padding-right:40px;padding-bottom:5px;padding-left:40px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">Здравствуйте, {userM.NickName}! Вы отправили запрос&nbsp;</p> <p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">на восстановление пароля?&nbsp;</p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">​</p></td></tr><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:5px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">Кто-то (надеемся, что вы) попросил нас сбросить пароль для вашей</p> <p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">&nbsp;учетной записи <a style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#38761d;font-size:14px;Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;text-align:center\" target=\"_blank\" href=\"https://hw.hw-online.front.freydin.space/products\">HW-Online</a>. Чтобы сделать это, щелкните по кнопке ниже.</p>\r\n<p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">&nbsp;Если вы не запрашивали сброс пароля, игнорируйте это сообщение!</p> <p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px;text-align:center\">​</p></td></tr> <tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><span class=\"es-button-border\" style=\"border-style:solid;border-color:#2CB543;background:#349d1f;border-width:0px;display:inline-block;border-radius:6px;width:auto;font-size:18px\"><a href='{config.GetValue<string>("Url")}?userId={userM.Id}&code={encode}' class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;color:#FFFFFF;font-size:18px;padding:10px 30px 10px 30px;display:inline-block;background:#349d1f;border-radius:6px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:24px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #349d1f;border-left-width:30px;border-right-width:30px\">Сброс пароля</a></span></td></tr> <tr>\r\n<td align=\"center\" class=\"es-m-p0r es-m-p0l\" style=\"Margin:0;padding-top:5px;padding-right:40px;padding-bottom:5px;padding-left:40px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:28px !important;letter-spacing:0;color:#333333;font-size:14px\">​</p></td></tr> <tr><td align=\"center\" style=\"padding:0;Margin:0;font-size:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:10px\"><a target=\"_blank\" href=\"https://vk.com/hwtpu\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img title=\"Собственная иконка\" src=\"https://fcmcbob.stripocdn.email/content/guids/CABINET_00b1de74d065a08edd1917864384386942a586c96ca49f166e096bb8b6c24ca3/images/_u2egx24fgm.jpg\" alt=\"Собственная иконка\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td>\r\n <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:10px\"><a target=\"_blank\" href=\"https://t.me/hwtpu\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img title=\"Telegram\" src=\"https://fcmcbob.stripocdn.email/content/assets/img/messenger-icons/logo-black/telegram-logo-black.png\" alt=\"Telegram\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0\"><a target=\"_blank\" href=\"https://www.youtube.com/@hwtpu_channel\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\"><img title=\"Youtube\" src=\"https://fcmcbob.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png\" alt=\"Yt\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td></tr></table> </td></tr></table></td>\r\n</tr></table></td></tr></table></td></tr></table></td></tr></table></div></body></html>");
                 //   $"Ваш запрос на сброс пароля получен.<br />Пожалуйста, посетите следующий URL-адрес, чтобы сбросить пароль: <a href='{config.GetValue<string>("Url")}?userId={userM.Id}&code={encode}'>link</a>");
            }
            catch (Exception e)
            {
                throw new BadRequestException(e.Message);
            }
        }
        else
        {
            throw new BadRequestException("User not found or email is not confirmed.");
        }
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="model"></param>
    public async Task ResetPasswordAsync(ResetPasswordDto model)
    {
        var userM = await _userManager.FindByEmailAsync(model.Email);
        if (userM  == null)
        {
            throw new NotFoundException("User was not found.");
        }
        
        var result = 
            await _userManager.ResetPasswordAsync(userM, HttpUtility.UrlDecode(model.Token), model.NewPassword);
        if (!result.Succeeded)
        {
            throw new ConflictException(string.Join(", ", result.Errors.Select(x => x.Description)));
        };
    }
    public async Task ConfirmEmail(Guid userId, string code) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var result =
            await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded) {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string jwtToken) {
        var key = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty));

        var validationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _configuration.GetSection("Jwt")["Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration.GetSection("Jwt")["Audience"],
            ValidateLifetime = false
        };

        ClaimsPrincipal principal;
        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
        }
        catch (ArgumentException ex) {
            throw new BadRequestException("Invalid jwt token", ex);
        }

        return principal;
    }

    private async Task<ClaimsIdentity?> GetIdentity(string email, string password) {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) return null;

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Id.ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user)) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsIdentity(claims, "Token", ClaimTypes.Name, ClaimTypes.Role);
    }
}
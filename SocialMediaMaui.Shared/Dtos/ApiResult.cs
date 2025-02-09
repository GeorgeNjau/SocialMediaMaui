﻿namespace SocialMediaMaui.Shared.Dtos
{
    public record ApiResult(bool isSuccess, string? Error)
    {
        public static ApiResult Success() => new(true, null);

        public static ApiResult Fail(string errorMessage) => new(false, errorMessage);
    }

    public record ApiResult<TData>(bool isSuccess, TData Data, string? Error)
    {
        public static ApiResult<TData> Success(TData data) => new(true, data, null);

        public static ApiResult<TData> Fail(string errorMessage) => new(false, default!, errorMessage);
    }

}

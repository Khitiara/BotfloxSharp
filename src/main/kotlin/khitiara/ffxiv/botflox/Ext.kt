package khitiara.ffxiv.botflox

import net.dv8tion.jda.api.requests.RestAction
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException
import kotlin.coroutines.suspendCoroutine

fun String.toSlug() = toLowerCase()
    .replace("\n", " ")
    .replace("[^a-z\\d\\s]".toRegex(), " ")
    .split(" ")
    .joinToString("-")
    .replace("-+".toRegex(), "-")

suspend fun <T> RestAction<T>.await() = suspendCoroutine<T> { continuation ->
    queue(
        continuation::resume,
        continuation::resumeWithException
    )
}
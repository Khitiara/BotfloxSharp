package khitiara.ffxiv.gearmanagement

import io.lettuce.core.RedisURI
import org.springframework.data.redis.connection.RedisPassword
import org.springframework.data.redis.connection.RedisStandaloneConfiguration

fun String.toSlug() = toLowerCase()
    .replace("\n", " ")
    .replace("[^a-z\\d\\s]".toRegex(), " ")
    .split(" ")
    .joinToString("-")
    .replace("-+".toRegex(), "-")

fun RedisURI.toSpringConfig(): RedisStandaloneConfiguration =
    RedisStandaloneConfiguration(host, port).also { it.password = RedisPassword.of(password) }